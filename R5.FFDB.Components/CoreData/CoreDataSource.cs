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
	public interface IMapper<TIn, TOut>
	{
		TOut Map(TIn input);
	}

	public interface ICoreDataSource<TCoreData>
	{
		Task<TCoreData> GetAsync(WeekInfo week);
	}
	
	// TVersionedModel should represent whatever a week's worth of this source data is
	//  - eg if we're collecting a list of stats for a week, the model should contain a list of the stats
	// similarly, TCoreData should represent whatever represents a weeks worth of the core data
	//  in a lot of cases, its gonna be a list of something
	public abstract class CoreDataSource<TVersionedModel, TCoreData> : ICoreDataSource<TCoreData>
		where TVersionedModel : class
		where TCoreData : class
	{
		protected DataDirectoryPath DataPath { get; }

		private IMapper<string, TVersionedModel> _toVersionedMapper { get; }
		private IMapper<TVersionedModel, TCoreData> _toCoreDataMapper { get; }
		private ProgramOptions _programOptions { get; }
		private IDatabaseProvider _dbProvider { get; }
		private IWebRequestClient _webClient { get; }

		protected CoreDataSource(
			IMapper<string, TVersionedModel> toVersionedMapper,
			IMapper<TVersionedModel, TCoreData> toCoreDataMapper,
			ProgramOptions programOptions,
			IDatabaseProvider dbProvider,
			DataDirectoryPath dataPath,
			IWebRequestClient webClient)
		{
			_toVersionedMapper = toVersionedMapper;
			_toCoreDataMapper = toCoreDataMapper;
			_programOptions = programOptions;
			_dbProvider = dbProvider;
			DataPath = dataPath;
			_webClient = webClient;
		}

		// 1. try get versioned from disk
		// 2. else, fetch from source (web)
		//    2b. if opts.SaveToDisk and SupportsFilePErsistence, SAVE
		public async Task<TCoreData> GetAsync(WeekInfo week)
		{
			TVersionedModel versioned = null;
			
			if (!TryGetVersionedFromDisk(week, out versioned))
			{
				versioned = await FetchFromSourceAsync(week);
			}

			return _toCoreDataMapper.Map(versioned);
		}
		
		protected abstract bool SupportsFilePersistence { get; }
		protected abstract string GetVersionedFilePath(WeekInfo week);
		protected abstract string GetSourceUri(WeekInfo week);

		private bool TryGetVersionedFromDisk(WeekInfo week, out TVersionedModel versioned)
		{
			versioned = null;

			if (!SupportsFilePersistence)
			{
				return false;
			}

			string filePath = GetVersionedFilePath(week);
			if (!File.Exists(filePath))
			{
				return false;
			}

			versioned = JsonConvert.DeserializeObject<TVersionedModel>(File.ReadAllText(filePath));
			return true;
		}

		private async Task<TVersionedModel> FetchFromSourceAsync(WeekInfo week)
		{
			string uri = GetSourceUri(week);

			string response = await _webClient.GetStringAsync(uri, throttle: false);

			TVersionedModel versioned = _toVersionedMapper.Map(response);

			if (SupportsFilePersistence && _programOptions.SaveToDisk)
			{
				string filePath = GetVersionedFilePath(week);

				string serializedModel = JsonConvert.SerializeObject(versioned);

				File.WriteAllText(filePath, serializedModel);
			}

			return versioned;
		}
	}

}
