using R5.FFDB.Components;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.Core.Models;
using R5.Lib.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Engine.NewProcessors
{
	public class StatsProcessor
	{
		private IDatabaseProvider _dbProvider { get; }
		//private IWeekGameDataCache _gameInfoCache { get; }
		private DataDirectoryPath _dataPath { get; }


		public StatsProcessor(
			IDatabaseProvider dbProvider,
			//IWeekGameDataCache gameInfoCache,
			DataDirectoryPath dataPath)
		{
			_dbProvider = dbProvider;
			//_gameInfoCache = gameInfoCache;
			_dataPath = dataPath;
		}

		// todo: move context, create pipeline factory
		public class AddStatsContext
		{

		}

		public async Task AddForWeekAsync(WeekInfo week)
		{
			//var pipeline = new AsyncPipeline<object>()
			//	// check if already updated
			//	.Next(async context =>
			//	{
			//		IDatabaseContext dbContext = _dbProvider.GetContext();

			//		bool alreadyUpdated = await dbContext.Log.HasUpdatedWeekAsync(week);
			//		if (alreadyUpdated)
			//		{
			//			//_logger.LogWarning($"Stats for {week} have already been added. Remove them first before try again.");
			//			return ProcessResult.End;
			//		}

			//		return ProcessResult.Continue;
			//	})
			//	.Next(async context =>
			//	{
			//		await _gameInfoCache.GetAsync(week);
			//		return ProcessResult.Continue;
			//	})
			//	// read from disk or fetch week stats
			//	.Next(async context =>
			//	{
			//		string statsJson;

			//		string filePath = _dataPath.Static.WeekStats + $"{week.Season}-{week.Week}.json";
			//		if (File.Exists(filePath))
			//		{
			//			statsJson = File.ReadAllText(filePath);
			//		}
			//		else
			//		{

			//		}

			//		var json = JsonConvert.DeserializeObject<WeekStatsJson>();
			//	})
			//	// get nfl player ids
			//	.Next(async context =>
			//	{

			//	})
			//	// 1a start fetching/persisting those - SAME TIME
			//	.Next(async context =>
			//	{

			//	})
			//	// 1b scrape target info for both players - CONCURRENTs
			//	.Next(async context =>
			//	{

			//	})
			//	// map to final model
			//	.Next(async context =>
			//	{

			//	})
		}
	}
}
