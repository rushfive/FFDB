using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.PlayerProfile;
using R5.FFDB.Components.CoreData.Roster.Values;
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
	public class UpdateProcessor
	{
		private ILogger<UpdateProcessor> _logger { get; }
		private IDatabaseProvider _dbProvider { get; }
		private RostersValue _rostersValue { get; }
		private AvailableWeeksValue _availableWeeksValue { get; }
		private IPlayerProfileSource _profileSource { get; }
		private IPlayerProfileService _profileService { get; }
		private ITeamGameHistorySource _teamGameHistorySource { get; }
		private IWeekStatsSource _weekStatsSource { get; }
		private IWeekStatsService _weekStatsService { get; }
		private ITeamGameStatsService _teamStatsService { get; }

		public UpdateProcessor(
			ILogger<UpdateProcessor> logger,
			IDatabaseProvider dbProvider,
			RostersValue rostersValue,
			AvailableWeeksValue availableWeeksValue,
			IPlayerProfileSource profileSource,
			IPlayerProfileService profileService,
			ITeamGameHistorySource teamGameHistorySource,
			IWeekStatsSource weekStatsSource,
			IWeekStatsService weekStatsService,
			ITeamGameStatsService teamStatsService)
		{
			_logger = logger;
			_dbProvider = dbProvider;
			_rostersValue = rostersValue;
			_availableWeeksValue = availableWeeksValue;
			_profileSource = profileSource;
			_profileService = profileService;
			_teamGameHistorySource = teamGameHistorySource;
			_weekStatsSource = weekStatsSource;
			_weekStatsService = weekStatsService;
			_teamStatsService = teamStatsService;
		}

		public async Task UpdateAllStatsAsync()
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

		public async Task UpdateMissingStatsAsync()
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

		public async Task UpdateStatsForWeekAsync(WeekInfo week)
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
			await AddPlayerProfilesAsync(weekStatNflIds, dbContext);

			WeekStats weekStats = await _weekStatsService.GetForWeekAsync(week);
			await dbContext.Stats.UpdateWeekAsync(weekStats);

			List<TeamWeekStats> teamStats = _teamStatsService.GetForWeek(week);
			await dbContext.Team.UpdateGameStatsAsync(teamStats);

			await dbContext.AddUpdateLogAsync(week);

		}

		public async Task UpdateRostersAsync()
		{
			IDatabaseContext dbContext = _dbProvider.GetContext();
			List<Roster> rosters = await _rostersValue.GetAsync();

			List<string> rosterNflIds = rosters.SelectMany(r => r.Players).Select(p => p.NflId).ToList();
			await AddPlayerProfilesAsync(rosterNflIds, dbContext);
			
			await dbContext.Team.UpdateRostersAsync(rosters);
		}

		private async Task AddPlayerProfilesAsync(List<string> nflIds, IDatabaseContext dbContext)
		{
			HashSet<string> existingIds = (await dbContext.Player.GetAllAsync())
				.Select(p => p.NflId)
				.ToHashSet();

			List<string> newIds = nflIds.Where(id => !existingIds.Contains(id)).ToList();
			if (!newIds.Any())
			{
				_logger.LogInformation($"No new player profiles to add. "
					+ $"The {nflIds.Count} players already exist in the database.");
				return;
			}

			_logger.LogInformation($"Adding {newIds.Count} player profiles to database.");

			await _profileSource.FetchAsync(newIds);

			List<PlayerProfile> playerProfiles = _profileService.Get(newIds);
			List<Roster> rosters = await _rostersValue.GetAsync();

			await dbContext.Player.UpdateAsync(playerProfiles, rosters);
		}
	}
}
