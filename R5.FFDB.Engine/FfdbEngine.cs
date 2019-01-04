using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.PlayerProfile;
using R5.FFDB.Components.CoreData.Roster;
using R5.FFDB.Components.CoreData.TeamGameHistory;
using R5.FFDB.Components.CoreData.WeekStats;
using R5.FFDB.Core.Models;
using R5.FFDB.Database;
using R5.FFDB.Engine.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R5.FFDB.Engine
{
	/*   Engine API notes
	 *   
		Misc. Engine stuff
		--- initial setup
		--- get status (for various things, eg latest updated week, timestamps for latest updates on rosters, etc)

		Teams
		--- update existing rosters by current team pages (includes fetching new player profiles)
		--- roadmap: depth charts (via rotoworld? would require a way to match players/rotoworldIds to Nfl ids 
		        (search/match on names, excluding punctuations? use some confidence rating, doesnt have to match every char but like 90% or something)
		--- roadmap: get and update team pictures

		WeekStats
		--- fetch & save all available (includes fetching new player profiles)

		Players
		--- update profile pictures 
		      player data currently has a static link, the link can be derived using the esbId

	 */
	public class FfdbEngine
	{
		private ILogger<FfdbEngine> _logger { get; }
		private CoreDataSourcesResolver _sourcesResolver { get; }
		private IDatabaseProvider _databaseProvider { get; }
		private IWeekStatsService _weekStatsService { get; }
		private IRosterService _rosterService { get; }
		private IGameStatsParser _gameStatsParser { get; }
		private IPlayerProfileService _playerProfileService { get; }
		private ITeamGameStatsService _teamGameStatsService { get; }

		public FfdbEngine(
			ILogger<FfdbEngine> logger,
			CoreDataSourcesResolver sourcesResolver,
			IDatabaseProvider databaseProvider,
			IWeekStatsService weekStatsService,
			IRosterService rosterService,
			IGameStatsParser gameStatsParser,
			IPlayerProfileService playerProfileService,
			ITeamGameStatsService teamGameStatsService)
		{
			_logger = logger;
			_sourcesResolver = sourcesResolver;
			_databaseProvider = databaseProvider;
			_weekStatsService = weekStatsService;
			_rosterService = rosterService;
			_gameStatsParser = gameStatsParser;
			_playerProfileService = playerProfileService;
			_teamGameStatsService = teamGameStatsService;
		}

		// can be run more than once, in case of failure
		public async Task RunInitialSetupAsync()
		{
			_logger.LogInformation("Running initial setup..");

			IDatabaseContext dbContext = _databaseProvider.GetContext();
			_logger.LogInformation($"Will run using database provider '{_databaseProvider.GetType().Name}'.");
			
			await dbContext.InitializeAsync();
			await dbContext.Team.AddTeamsAsync();

			CoreDataSources sources = await _sourcesResolver.GetAsync();

			//await sources.TeamGameHistory.FetchAndSaveAsync();
			
			_gameStatsParser.ParseFilesToMapValues();

			//await sources.Roster.FetchAndSaveAsync();
			List<Roster> rosters = _rosterService.Get();

			//await sources.PlayerProfile.FetchAndSaveAsync();

			_logger.LogInformation("Persisting player profiles to database..");
			
			List<PlayerProfile> players = _playerProfileService.Get();
			await dbContext.Player.AddAsync(players, rosters);

			_logger.LogInformation("Persisting player-team mappings (roster) to database..");
			await dbContext.Team.UpdateRostersAsync(rosters);

			//await sources.WeekStats.FetchAndSaveAsync();
			List<WeekStats> weekStats = (await _weekStatsService.GetAsync())
				.OrderBy(ws => ws.Week)
				.ToList();

			_logger.LogInformation("Persisting week stats to database..");
			await dbContext.Stats.UpdateWeeksAsync(weekStats);
			
			List<TeamWeekStats> teamGameStats = _teamGameStatsService.Get();
			await dbContext.Team.UpdateGameStatsAsync(teamGameStats);

			_logger.LogInformation("Successfully finished running initial setup.");
		}

		public async Task UpdateForWeekAsync(int season, int week)
		{
			_logger.LogInformation($"Starting update for {season}-{week}.");


		}
	}
}
