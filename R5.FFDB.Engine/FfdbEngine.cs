using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.PlayerData;
using R5.FFDB.Components.PlayerData.Sources.NFLWebPlayerProfile;
using R5.FFDB.Components.Roster;
using R5.FFDB.Components.WeekStats;
using R5.FFDB.Components.WeekStats.Sources.NFLFantasyApi;
using System.Collections.Generic;

namespace R5.FFDB.Engine
{
	public class FfdbEngine
	{
		private ILogger<FfdbEngine> _logger { get; }
		private IPlayerDataSource _playerDataService { get; }
		private IRosterSource _rosterService { get; }
		private IWeekStatsSource _weekStatsService { get; }

		// leave as public for now,
		// but should be configured with builder
		public FfdbEngine(
			ILogger<FfdbEngine> logger,
			IPlayerDataSource playerDataService,
			IRosterSource rosterService,
			IWeekStatsSource weekStatsService)
		{
			_logger = logger;
			_playerDataService = playerDataService;
			_rosterService = rosterService;
			_weekStatsService = weekStatsService;
			
		}

		public void TestLogging()
		{
			_logger.LogTrace("this is a trace log.");
			_logger.LogDebug("this is a debug log");
			_logger.LogCritical("this is a criterial log");
		}
	}

	// todo: move
	//public class PlayerDataSource
	//{
	//	private List<IPlayerDataSource> _sources { get; } = new List<IPlayerDataSource>();

	//	public PlayerDataSource()
	//	{

	//	}
	//}
}
