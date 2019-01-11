using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData;
using R5.FFDB.Components.CoreData.PlayerProfile;
using R5.FFDB.Components.CoreData.Roster;
using R5.FFDB.Components.CoreData.TeamGameHistory;
using R5.FFDB.Components.CoreData.WeekStats;
using R5.FFDB.Core.Models;
using R5.FFDB.Database;
using R5.FFDB.Database.DbContext;
using R5.FFDB.Engine.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R5.FFDB.Engine
{
	public class FfdbEngine
	{
		public StatsProcessor Stats { get; }
		public TeamProcessor Team { get; }
		public PlayerProcessor Player { get; }

		private ILogger<FfdbEngine> _logger { get; }
		private IDatabaseProvider _databaseProvider { get; }
		private List<ICoreDataSource> _coreDataSources { get; }

		public FfdbEngine(
			ILogger<FfdbEngine> logger,
			IDatabaseProvider databaseProvider,
			IPlayerProfileSource playerProfileSource,
			IRosterSource rosterSource,
			IWeekStatsSource weekStatsSource,
			ITeamGameHistorySource teamGameHistorySource,
			IServiceProvider serviceProvider)
		{
			_logger = logger;
			_databaseProvider = databaseProvider;

			_coreDataSources = new List<ICoreDataSource>
			{
				playerProfileSource,
				rosterSource,
				weekStatsSource,
				teamGameHistorySource
			};

			Stats = ActivatorUtilities.CreateInstance<StatsProcessor>(serviceProvider);
			Team = ActivatorUtilities.CreateInstance<TeamProcessor>(serviceProvider);
			Player = ActivatorUtilities.CreateInstance<PlayerProcessor>(serviceProvider);
		}

		// TODO: optional FORCE parameter that wil allow re-running
		// should obvioulsy remove everything existing first
		// postgres -> drop schema IF EXISTS with CASCADE
		// mongo -> drop all collections (indexes are sub items)
		public async Task RunInitialSetupAsync()
		{
			_logger.LogInformation("Running initial setup..");

			await CheckSourcesHealthAsync();

			IDatabaseContext dbContext = _databaseProvider.GetContext();
			_logger.LogInformation($"Will run using database provider '{_databaseProvider.GetType().Name}'.");

			await dbContext.InitializeAsync(force: true);
			await dbContext.Team.AddTeamsAsync();

			await Stats.AddMissingAsync();
			await Team.UpdateRostersAsync();

			_logger.LogInformation("Successfully finished running initial setup.");
		}

		public async Task CheckSourcesHealthAsync()
		{
			_logger.LogInformation("Starting health checks for all core data sources.");

			foreach (ICoreDataSource source in _coreDataSources)
			{
				await source.CheckHealthAsync();
			}

			_logger.LogInformation("All health checks successfully passed.");
		}
	}
}
