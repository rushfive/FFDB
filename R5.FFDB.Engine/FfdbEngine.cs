using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R5.FFDB.Components.CoreData;
using R5.FFDB.Components.CoreData.Players;
using R5.FFDB.Components.CoreData.Rosters;
using R5.FFDB.Components.CoreData.TeamGames;
//using R5.FFDB.Components.CoreData.WeekStats;
using R5.FFDB.Components.Extensions.JsonConverters;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.Core.Models;
using R5.FFDB.Engine.Processors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R5.FFDB.Engine
{
	public class FfdbEngine
	{
		public StatsProcessor Stats { get; private set; }
		public TeamProcessor Team { get; private set; }
		public PlayerProcessor Player { get; private set; }

		private ILogger<FfdbEngine> _logger { get; }
		private IDatabaseProvider _databaseProvider { get; }
		//private List<ICoreDataSource> _coreDataSources { get; set; }
		private LatestWeekValue _latestWeekValue { get; }

		public FfdbEngine(
			ILogger<FfdbEngine> logger,
			IDatabaseProvider databaseProvider,
			LatestWeekValue latestWeekValue,
			IServiceProvider serviceProvider)
		{
			_logger = logger;
			_databaseProvider = databaseProvider;
			_latestWeekValue = latestWeekValue;

			InitializeSourcesProcessors(serviceProvider);
		}

		static FfdbEngine()
		{
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings
			{
				Formatting = Formatting.None,
				Converters = new List<JsonConverter>
				{
					new WeekInfoJsonConverter()
				}
			};
		}

		private void InitializeSourcesProcessors(IServiceProvider serviceProvider)
		{
			//_coreDataSources = new List<ICoreDataSource>
			//{
			//	serviceProvider.GetRequiredService<IPlayerSource>(),
			//	serviceProvider.GetRequiredService<IRosterSource>(),
			//	//serviceProvider.GetRequiredService<IWeekStatsSource>(),
			//	serviceProvider.GetRequiredService<ITeamGamesSource>()
			//};

			Stats = ActivatorUtilities.CreateInstance<StatsProcessor>(serviceProvider);
			Team = ActivatorUtilities.CreateInstance<TeamProcessor>(serviceProvider);
			Player = ActivatorUtilities.CreateInstance<PlayerProcessor>(serviceProvider);
		}

		public async Task RunInitialSetupAsync(bool forceReinitialize)
		{
			_logger.LogInformation("Running initial setup..");

			await CheckSourcesHealthAsync();

			IDatabaseContext dbContext = _databaseProvider.GetContext();
			_logger.LogInformation($"Will run using database provider '{_databaseProvider.GetType().Name}'.");

			await dbContext.InitializeAsync(forceReinitialize);
			await dbContext.Team.AddTeamsAsync();

			await Stats.AddMissingAsync();
			await Team.UpdateRosterMappingsAsync();

			_logger.LogInformation("Successfully finished running initial setup.");
		}

		public async Task CheckSourcesHealthAsync()
		{
			_logger.LogInformation("Starting health checks for all core data sources.");

			//foreach (ICoreDataSource source in _coreDataSources)
			//{
			//	await source.CheckHealthAsync();
			//}

			_logger.LogInformation("All health checks successfully passed.");
		}

		public Task<bool> HasBeenInitializedAsync()
		{
			IDatabaseContext dbContext = _databaseProvider.GetContext();
			return dbContext.HasBeenInitializedAsync();
		}

		public Task<WeekInfo> GetLatestWeekAsync()
		{
			return _latestWeekValue.GetAsync();
		}

		public Task<List<WeekInfo>> GetAllUpdatedWeeksAsync()
		{
			IDatabaseContext dbContext = _databaseProvider.GetContext();
			return dbContext.Log.GetUpdatedWeeksAsync();
		}
	}
}
