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
	public interface IAsyncMapper<TIn, TOut, TSourceKey>
	{
		Task<TOut> MapAsync(TIn input, TSourceKey sourceKey);
	}

	public interface ICoreDataSource<TCoreData, TKey>
	{
		Task<TCoreData> GetAsync(TKey key);
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
		[Obsolete("dont think we need dbContext for sources")]
		private IDatabaseProvider _dbProvider { get; }
		private IWebRequestClient _webClient { get; }

		protected CoreDataSource(
			ILogger<CoreDataSource<TVersionedModel, TCoreData, TKey>> logger,
			IAsyncMapper<string, TVersionedModel, TKey> toVersionedMapper,
			IAsyncMapper<TVersionedModel, TCoreData, TKey> toCoreMapper,
			ProgramOptions programOptions,
			IDatabaseProvider dbProvider,
			DataDirectoryPath dataPath,
			IWebRequestClient webClient)
		{
			_logger = logger;
			_toVersionedMapper = toVersionedMapper;
			_toCoreMapper = toCoreMapper;
			_programOptions = programOptions;
			_dbProvider = dbProvider;
			DataPath = dataPath;
			_webClient = webClient;
		}
		
		public async Task<TCoreData> GetAsync(TKey key)
		{
			TVersionedModel versioned = null;
			
			if (!TryGetVersionedFromDisk(key, out versioned))
			{
				versioned = await FetchFromSourceAsync(key);
			}

			TCoreData coreData = await _toCoreMapper.MapAsync(versioned, key);

			await OnCoreDataMappedAsync(key, coreData);

			return coreData;
		}
		
		protected abstract bool SupportsFilePersistence { get; }
		protected abstract string GetVersionedFilePath(TKey key);
		protected abstract string GetSourceUri(TKey key);

		[Obsolete("replaced by adding source key to mappers")]
		protected abstract Task OnVersionedModelMappedAsync(TKey key, TVersionedModel versioned);
		[Obsolete("replaced by adding source key to mappers")]
		protected abstract Task OnCoreDataMappedAsync(TKey key, TCoreData coreData);

		private bool TryGetVersionedFromDisk(TKey key, out TVersionedModel versioned)
		{
			versioned = null;

			if (!SupportsFilePersistence)
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

		private async Task<TVersionedModel> FetchFromSourceAsync(TKey key)
		{
			string uri = GetSourceUri(key);
			string response = await _webClient.GetStringAsync(uri, throttle: false);

			TVersionedModel versioned = await _toVersionedMapper.MapAsync(response, key);

			await OnVersionedModelMappedAsync(key, versioned);

			if (SupportsFilePersistence && _programOptions.SaveToDisk)
			{
				string filePath = GetVersionedFilePath(key);

				string serializedModel = JsonConvert.SerializeObject(versioned);

				File.WriteAllText(filePath, serializedModel);
			}

			return versioned;
		}
	}

}
