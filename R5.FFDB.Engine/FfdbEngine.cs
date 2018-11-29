using Microsoft.Extensions.Logging;
using R5.FFDB.Engine.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R5.FFDB.Engine
{
	public class FfdbEngine
	{
		private ILogger<FfdbEngine> _logger { get; }
		private ISourcesFactory _sourcesFactory { get; }

		public FfdbEngine(
			ILogger<FfdbEngine> logger,
			ISourcesFactory sourcesFactory)
		{
			_logger = logger;
			_sourcesFactory = sourcesFactory;
		}
		
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

			Sources sources = await _sourcesFactory.GetAsync();

			// get all available week stats
			try
			{
				// todo: it should actually ALWAYS FETCH from web for this initial setup
				//var rosters = await sources.Roster.GetFromWebAsync(saveToDisk: false);
				List<Core.Models.Roster> rosters = sources.Roster.GetFromDisk();

				List<string> rosterPlayerIds = rosters
					.SelectMany(r => r.Players)
					.Select(p => p.NflId)
					.Distinct()
					//.Select(p => (p.NflId, p.FirstName, p.LastName))
					.ToList();

				_logger.LogInformation($"Found '{rosterPlayerIds.Count}' players from rosters to fetch profile data for.");
				await sources.PlayerProfile.FetchAndSavePlayerDataFilesAsync(rosterPlayerIds);
				_logger.LogInformation("Finished fetching player profile data by rosters.");

				await sources.WeekStats.FetchAndSaveWeekStatsAsync();

				List<Core.Models.WeekStats> weekStats = sources.WeekStats.GetAll();

				List<string> weekStatsPlayerIds = weekStats
					.SelectMany(ws => ws.Players)
					.Select(p => p.NflId)
					.Distinct()
					.ToList();

				_logger.LogInformation($"Found '{weekStatsPlayerIds.Count}' players from week stats to fetch profile data for.");
				await sources.PlayerProfile.FetchAndSavePlayerDataFilesAsync(weekStatsPlayerIds);
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
