using Newtonsoft.Json;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData
{
	public interface ICoreDataSource<TCoreData, TKey>
	{
		Task<SourceResult<TCoreData>> GetAsync(TKey key);
	}
	
	public abstract class CoreDataSource<TVersionedModel, TCoreData, TKey>
		where TVersionedModel : class
		where TCoreData : class
	{
		protected DataDirectoryPath DataPath { get; }

		private IAppLogger _logger { get; }
		private IAsyncMapper<string, TVersionedModel, TKey> _toVersionedMapper { get; }
		private IAsyncMapper<TVersionedModel, TCoreData, TKey> _toCoreMapper { get; }
		private ProgramOptions _programOptions { get; }
		private IWebRequestClient _webClient { get; }

		protected CoreDataSource(
			IAppLogger logger,
			IAsyncMapper<string, TVersionedModel, TKey> toVersionedMapper,
			IAsyncMapper<TVersionedModel, TCoreData, TKey> toCoreMapper,
			ProgramOptions programOptions,
			DataDirectoryPath dataPath,
			IWebRequestClient webClient)
		{
			_logger = logger;
			_toVersionedMapper = toVersionedMapper;
			_toCoreMapper = toCoreMapper;
			_programOptions = programOptions;
			DataPath = dataPath;
			_webClient = webClient;
		}

		public async Task<SourceResult<TCoreData>> GetAsync(TKey key)
		{
			TVersionedModel versioned = null;
			bool fetchedFromWeb = false;
			
			if (!TryGetVersionedFromDisk(key, out versioned))
			{
				if (SupportsDataRepoFetch && _programOptions.DataRepoEnabled)
				{
					versioned = await GetVersionedFromDataRepoAsync(key);
				}

				if (versioned != null)
				{
					if (SupportsVersionedFilePersistence && _programOptions.SaveToDisk)
					{
						string filePath = GetVersionedFilePath(key);

						string serializedModel = JsonConvert.SerializeObject(versioned);

						File.WriteAllText(filePath, serializedModel);
					}
				}
				else
				{
					var (versionedModel, fetchedWeb) = await FetchFromSourceAsync(key);

					versioned = versionedModel;
					fetchedFromWeb = fetchedWeb;
				}
			}

			TCoreData coreData = await _toCoreMapper.MapAsync(versioned, key);

			return new SourceResult<TCoreData>(coreData, fetchedFromWeb);
		}

		private async Task<TVersionedModel> GetVersionedFromDataRepoAsync(TKey key)
		{
			try
			{
				string uri = GetDataRepoUri(key);
				string response = await _webClient.GetStringAsync(uri, throttle: false);

				var versioned = JsonConvert.DeserializeObject<TVersionedModel>(response);

				_logger.LogDebug($"Fetched versioned model from data repo for '{key}'.");

				return versioned;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"There was an error fetching from the data repo for '{key}'.");
				return null;
			}
		}

		protected abstract bool SupportsSourceFilePersistence { get; }
		protected abstract bool SupportsVersionedFilePersistence { get; }
		protected abstract bool SupportsDataRepoFetch { get; }

		protected abstract string GetVersionedFilePath(TKey key);
		protected abstract string GetSourceFilePath(TKey key);
		protected abstract string GetSourceUri(TKey key);
		protected abstract string GetDataRepoUri(TKey key);

		private bool TryGetVersionedFromDisk(TKey key, out TVersionedModel versioned)
		{
			versioned = null;

			if (!SupportsVersionedFilePersistence)
			{
				return false;
			}

			string filePath = GetVersionedFilePath(key);
			if (!File.Exists(filePath))
			{
				return false;
			}

			versioned = JsonConvert.DeserializeObject<TVersionedModel>(File.ReadAllText(filePath));

			_logger.LogDebug("Retrieved versioned model from disk:"
				+ Environment.NewLine + "{@VersionedModel}", versioned);

			return true;
		}

		private async Task<(TVersionedModel versioned, bool fetchedFromWeb)> FetchFromSourceAsync(TKey key)
		{
			bool fetchedFromWeb = false;

			if (!TryGetSourceFile(key, out string sourceResponse))
			{
				string uri = GetSourceUri(key);
				sourceResponse = await _webClient.GetStringAsync(uri, throttle: false);

				// we dont currently log trace levels, this was outputting way too much
				// esp for the HTML page fetches
				_logger.LogTrace("Fetched source response from web:"
					+ Environment.NewLine + "{@SourceResponse}", sourceResponse);

				if (SupportsSourceFilePersistence && !File.Exists(GetSourceFilePath(key)))
				{
					File.WriteAllText(GetSourceFilePath(key), sourceResponse);
				}

				fetchedFromWeb = true;
			}

			TVersionedModel versioned = await _toVersionedMapper.MapAsync(sourceResponse, key);

			_logger.LogDebug("Mapped to versioned model:"
				+ Environment.NewLine + "{@VersionedModel}", versioned);

			if (SupportsVersionedFilePersistence && _programOptions.SaveToDisk)
			{
				string filePath = GetVersionedFilePath(key);

				string serializedModel = JsonConvert.SerializeObject(versioned);

				File.WriteAllText(filePath, serializedModel);
			}

			return (versioned, fetchedFromWeb);
		}

		private bool TryGetSourceFile(TKey key, out string sourceResponse)
		{
			sourceResponse = null;

			if (!SupportsSourceFilePersistence)
			{
				return false;
			}

			string filePath = GetSourceFilePath(key);
			if (!File.Exists(filePath))
			{
				return false;
			}

			sourceResponse = File.ReadAllText(filePath);

			_logger.LogDebug("Retrieved source response from disk:"
				+ Environment.NewLine + "{@SourceReponse}", sourceResponse);

			return true;
		}
	}

	public interface IAsyncMapper<TIn, TOut, TSourceKey>
	{
		Task<TOut> MapAsync(TIn input, TSourceKey sourceKey);
	}

	public class SourceResult<TCoreData>
	{
		public TCoreData Value { get; }
		public bool FetchedFromWeb { get; }

		public SourceResult(
			TCoreData value,
			bool fetchedFromWeb)
		{
			Value = value;
			FetchedFromWeb = fetchedFromWeb;
		}
	}
}
