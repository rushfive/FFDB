using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.Http;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData
{
	public interface ICoreDataSource<TCoreData, TKey>
	{
		Task<SourceResult<TCoreData>> GetAsync(TKey key);
	}

	// TVersionedModel should represent whatever a week's worth of this source data is
	//  - eg if we're collecting a list of stats for a week, the model should contain a list of the stats
	// similarly, TCoreData should represent whatever represents a weeks worth of the core data
	//  in a lot of cases, its gonna be a list of something
	public abstract class CoreDataSource<TVersionedModel, TCoreData, TKey>
		where TVersionedModel : class
		where TCoreData : class
	{
		protected DataDirectoryPath DataPath { get; }

		private ILogger<CoreDataSource<TVersionedModel, TCoreData, TKey>> _logger { get; }
		private IAsyncMapper<string, TVersionedModel, TKey> _toVersionedMapper { get; }
		private IAsyncMapper<TVersionedModel, TCoreData, TKey> _toCoreMapper { get; }
		private ProgramOptions _programOptions { get; }
		private IWebRequestClient _webClient { get; }

		protected CoreDataSource(
			ILogger<CoreDataSource<TVersionedModel, TCoreData, TKey>> logger,
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
				var (versionedModel, fetchedWeb) = await FetchFromSourceAsync(key);

				versioned = versionedModel;
				fetchedFromWeb = fetchedWeb;
			}

			TCoreData coreData = await _toCoreMapper.MapAsync(versioned, key);

			return new SourceResult<TCoreData>(coreData, fetchedFromWeb);
		}

		protected abstract bool SupportsSourceFilePersistence { get; }
		protected abstract bool SupportsVersionedFilePersistence { get; }

		protected abstract string GetVersionedFilePath(TKey key);
		protected abstract string GetSourceFilePath(TKey key);
		protected abstract string GetSourceUri(TKey key);

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
			return true;
		}

		private async Task<(TVersionedModel versioned, bool fetchedFromWeb)> FetchFromSourceAsync(TKey key)
		{
			bool fetchedFromWeb = false;

			if (!TryGetSourceFile(key, out string sourceResponse))
			{
				string uri = GetSourceUri(key);
				sourceResponse = await _webClient.GetStringAsync(uri, throttle: false);

				if (SupportsSourceFilePersistence && !File.Exists(GetSourceFilePath(key)))
				{
					File.WriteAllText(GetSourceFilePath(key), sourceResponse);
				}

				fetchedFromWeb = true;
			}

			TVersionedModel versioned = await _toVersionedMapper.MapAsync(sourceResponse, key);

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
			return true;
		}
	}

}
