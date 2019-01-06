using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.TeamGameHistory;
using R5.FFDB.Components.CoreData.WeekStats;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core.Models;
using R5.FFDB.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Engine.Processors
{
	public class StatsProcessor
	{
		private ILogger<StatsProcessor> _logger { get; }
		private IDatabaseProvider _dbProvider { get; }
		private AvailableWeeksValue _availableWeeksValue { get; }
		private ITeamGameHistorySource _teamGameHistorySource { get; }
		private IWeekStatsSource _weekStatsSource { get; }
		private IWeekStatsService _weekStatsService { get; }
		private ITeamGameStatsService _teamStatsService { get; }
		private IProcessorHelper _helper { get; }

		public StatsProcessor(
			ILogger<StatsProcessor> logger,
			IDatabaseProvider dbProvider,
			AvailableWeeksValue availableWeeksValue,
			ITeamGameHistorySource teamGameHistorySource,
			IWeekStatsSource weekStatsSource,
			IWeekStatsService weekStatsService,
			ITeamGameStatsService teamStatsService,
			IProcessorHelper helper)
		{
			_logger = logger;
			_dbProvider = dbProvider;
			_availableWeeksValue = availableWeeksValue;
			_teamGameHistorySource = teamGameHistorySource;
			_weekStatsSource = weekStatsSource;
			_weekStatsService = weekStatsService;
			_teamStatsService = teamStatsService;
			_helper = helper;
		}

		public async Task UpdateAllAsync()
		{
			List<WeekInfo> available = await _availableWeeksValue.GetAsync();

			_logger.LogInformation($"Updating stats for all available weeks ({available.Count} total).");
			
			foreach(var week in available)
			{
				_logger.LogDebug($"Begin updating stats for {week}.");
				await UpdateStatsForWeekInternalAsync(week);
				_logger.LogInformation($"Finished updating stats for {week}.");
			}

			_logger.LogInformation("Finished updating stats for all available weeks.");
		}

		public async Task UpdateMissingAsync()
		{
			IDatabaseContext dbContext = _dbProvider.GetContext();
			HashSet<WeekInfo> alreadyUpdated = (await dbContext.GetUpdatedWeeksAsync()).ToHashSet();

			List<WeekInfo> available = await _availableWeeksValue.GetAsync();

			List<WeekInfo> missing = available.Where(w => !alreadyUpdated.Contains(w)).ToList();
			if (!missing.Any())
			{
				_logger.LogInformation($"There are no missing weeks to update. A total of {available.Count} weeks of stats already exists.");
				return;
			}

			_logger.LogInformation($"Updating stats for {missing.Count} missing weeks.");
			
			foreach (var week in missing)
			{
				_logger.LogDebug($"Begin updating stats for {week}.");
				await UpdateStatsForWeekInternalAsync(week);
				_logger.LogInformation($"Finished updating stats for {week}.");
			}

			_logger.LogInformation("Finished updating stats for missing weeks.");
		}

		public async Task UpdateForWeekAsync(WeekInfo week)
		{
			_logger.LogInformation($"Updating stats for {week}.");

			await UpdateStatsForWeekInternalAsync(week);

			_logger.LogInformation($"Finished updating stats for {week}.");
		}

		private async Task UpdateStatsForWeekInternalAsync(WeekInfo week)
		{
			IDatabaseContext dbContext = _dbProvider.GetContext();

			await _teamGameHistorySource.FetchForWeekAsync(week);
			await _weekStatsSource.FetchForWeekAsync(week);

			List<string> weekStatNflIds = _weekStatsService.GetNflIdsForWeek(week);
			await _helper.AddPlayerProfilesAsync(weekStatNflIds, dbContext);

			WeekStats weekStats = await _weekStatsService.GetForWeekAsync(week);
			await dbContext.Stats.UpdateWeekAsync(weekStats);

			List<TeamWeekStats> teamStats = _teamStatsService.GetForWeek(week);
			await dbContext.Team.UpdateGameStatsAsync(teamStats);

			await dbContext.AddUpdateLogAsync(week);
		}
	}
}
