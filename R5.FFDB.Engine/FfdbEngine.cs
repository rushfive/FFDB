using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.PlayerData;
using R5.FFDB.Components.Roster;
using R5.FFDB.Components.WeekStats;
using System.Collections.Generic;

namespace R5.FFDB.Engine
{
	public class FfdbEngine
	{
		private ILogger<FfdbEngine> _logger { get; }
		private IPlayerDataService _playerDataService { get; }
		private IRosterService _rosterService { get; }
		private IWeekStatsService _weekStatsService { get; }

		// leave as public for now,
		// but should be configured with builder
		public FfdbEngine(
			ILogger<FfdbEngine> logger,
			IPlayerDataService playerDataService,
			IRosterService rosterService,
			IWeekStatsService weekStatsService)
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
	public class PlayerDataSource
	{
		private List<IPlayerDataService> _sources { get; } = new List<IPlayerDataService>();

		public PlayerDataSource()
		{

		}
	}
}
