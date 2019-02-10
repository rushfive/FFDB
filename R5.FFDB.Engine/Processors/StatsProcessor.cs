using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.TeamGames;
//using R5.FFDB.Components.CoreData.WeekStats;
using R5.FFDB.Components.Pipelines.Stats;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R5.FFDB.Engine.Processors
{
	public class StatsProcessor
	{
		private IServiceProvider _serviceProvider { get; }

		private ILogger<StatsProcessor> _logger { get; }
		private IDatabaseProvider _dbProvider { get; }
		private AvailableWeeksValue _availableWeeksValue { get; }
		private ITeamGamesSource _teamGamesSource { get; }
		//private IWeekStatsSource _weekStatsSource { get; }
		//private IWeekStatsService _weekStatsService { get; }
		private ITeamGameStatsService _teamStatsService { get; }
		private IProcessorHelper _helper { get; }
		//private IWeekGameMatchupService _gameMatchupService { get; }

		public StatsProcessor(
			IServiceProvider serviceProvider,

			ILogger<StatsProcessor> logger,
			IDatabaseProvider dbProvider,
			AvailableWeeksValue availableWeeksValue,
			ITeamGamesSource teamGamesSource,
			//IWeekStatsSource weekStatsSource,
			//IWeekStatsService weekStatsService,
			ITeamGameStatsService teamStatsService,
			IProcessorHelper helper
			//IWeekGameMatchupService gameMatchupService
			)
		{
			_serviceProvider = serviceProvider;

			_logger = logger;
			_dbProvider = dbProvider;
			_availableWeeksValue = availableWeeksValue;
			_teamGamesSource = teamGamesSource;
			//_weekStatsSource = weekStatsSource;
			//_weekStatsService = weekStatsService;
			_teamStatsService = teamStatsService;
			_helper = helper;
			//_gameMatchupService = gameMatchupService;
		}

		public async Task AddMissingAsync()
		{
			IDatabaseContext dbContext = _dbProvider.GetContext();
			HashSet<WeekInfo> alreadyUpdated = (await dbContext.Log.GetUpdatedWeeksAsync()).ToHashSet();

			List<WeekInfo> available = await _availableWeeksValue.GetAsync();

			List<WeekInfo> missing = available.Where(w => !alreadyUpdated.Contains(w)).ToList();
			if (!missing.Any())
			{
				_logger.LogInformation($"There are no missing weeks to add. A total of {available.Count} weeks of stats already exists.");
				return;
			}

			_logger.LogInformation($"Adding stats for {missing.Count} missing weeks.");

			foreach (var week in missing)
			{
				_logger.LogDebug($"Begin adding stats for {week}.");
				await AddStatsForWeekInternalAsync(week);
				_logger.LogInformation($"Finished adding stats for {week}.");
			}

			_logger.LogInformation("Finished adding stats for missing weeks.");
		}

		public async Task AddForWeekAsync(WeekInfo week)
		{
			_logger.LogInformation($"Adding stats for {week}.");

			IDatabaseContext dbContext = _dbProvider.GetContext();
			bool alreadyUpdated = await dbContext.Log.HasUpdatedWeekAsync(week);
			if (alreadyUpdated)
			{
				_logger.LogWarning($"Stats for {week} have already been added. Remove them first before try again.");
				return;
			}

			await AddStatsForWeekInternalAsync(week);

			_logger.LogInformation($"Finished adding stats for {week}.");
		}

		private async Task AddStatsForWeekInternalAsync(WeekInfo week)
		{
			IDatabaseContext dbContext = _dbProvider.GetContext();

			await _teamGamesSource.FetchForWeekAsync(week);//
			//await _weekStatsSource.FetchForWeekAsync(week);//

			//List<string> weekStatNflIds = _weekStatsService.GetNflIdsForWeek(week);//
			//await _helper.AddPlayerProfilesAsync(weekStatNflIds, dbContext);//

			//WeekStats weekStats = await _weekStatsService.GetForWeekAsync(week);//
			//await dbContext.Stats.AddWeekAsync(weekStats);

			List<TeamWeekStats> teamStats = _teamStatsService.GetForWeek(week); // team game data cache
			await dbContext.Team.AddGameStatsAsync(teamStats);

			/// INSTEAD: get this from the IWeekGameDataCache
			//List<WeekGameMatchup> gameMatchups = _gameMatchupService.GetForWeek(week); // game info cache
			//await dbContext.Team.AddGameMatchupsAsync(gameMatchups);

			await dbContext.Log.AddUpdateForWeekAsync(week);
		}

		public Task RemoveAllAsync()
		{
			var pipeline = RemoveAllPipeline.Create(_serviceProvider);

			return pipeline.ProcessAsync(null);

			//_logger.LogInformation("Removing all stats (WeekStats and TeamGameStats) and logs.");

			//IDatabaseContext dbContext = _dbProvider.GetContext();

			//await dbContext.Stats.RemoveAllAsync();
			//await dbContext.Team.RemoveAllGameStatsAsync();
			//await dbContext.Log.RemoveAllAsync();

			//_logger.LogInformation("Finished removing all stats and logs.");
		}

		public Task RemoveForWeekAsync(WeekInfo week)
		{
			var context = new RemoveWeekPipeline.Context { Week = week };

			var pipeline = RemoveWeekPipeline.Create(_serviceProvider);

			return pipeline.ProcessAsync(context);


			//_logger.LogInformation($"Removing stats (WeekStats and TeamGameStats) and logs for {week}.");

			//IDatabaseContext dbContext = _dbProvider.GetContext();

			//await dbContext.Stats.RemoveForWeekAsync(week);
			//await dbContext.Team.RemoveGameStatsForWeekAsync(week);
			//await dbContext.Log.RemoveForWeekAsync(week);

			//_logger.LogInformation($"Finished removing stats and logs for {week}.");
		}
	}
}
