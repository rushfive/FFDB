using Microsoft.Extensions.Logging;
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
		private SourcesResolver _sourcesResolver { get; }
		private IDatabaseProvider _databaseProvider { get; }

		public FfdbEngine(
			ILogger<FfdbEngine> logger,
			SourcesResolver sourcesResolver,
			IDatabaseProvider databaseProvider)
		{
			_logger = logger;
			_sourcesResolver = sourcesResolver;
			_databaseProvider = databaseProvider;
		}
		
		// can be run more than once, in case of failure
		public async Task RunInitialSetupAsync()
		{
			_logger.LogInformation("Running initial setup..");

			// todo:
			// 1. config validation
			// 2. intiialize DB
			// 3. verify/check sources (all must have AT LEAST one working source)
			// 4. get all latest weekstats
			// 5. download all rosters to temp
			// 6. fetch and save player profiles from ROSTERS first, WEEKSTATS next

			Sources sources = await _sourcesResolver.GetAsync();

			// get all available week stats
			try
			{
				List<Roster> rosters = await sources.Roster.GetAsync();

				List<string> rosterPlayerIds = rosters
					.SelectMany(r => r.Players)
					.Select(p => p.NflId)
					.Distinct()
					.ToList();

				_logger.LogInformation($"Found '{rosterPlayerIds.Count}' players from rosters to fetch profile data for.");
				await sources.PlayerProfile.FetchAndSaveAsync(rosterPlayerIds);
				_logger.LogInformation("Finished fetching player profile data by rosters.");

				await sources.WeekStats.FetchAndSaveAsync();

				List<WeekStats> weekStats = sources.WeekStats.GetAll();

				List<string> weekStatsPlayerIds = weekStats
					.SelectMany(ws => ws.Players)
					.Select(p => p.NflId)
					.Distinct()
					.ToList();

				_logger.LogInformation($"Found '{weekStatsPlayerIds.Count}' players from week stats to fetch profile data for.");
				await sources.PlayerProfile.FetchAndSaveAsync(weekStatsPlayerIds);
				_logger.LogInformation("Finished fetching player profile data by week stats.");

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "There was an error running the initial setup.");
				throw;
			}

			// fetch and save current team roster pages

		}
		
	}	
}
