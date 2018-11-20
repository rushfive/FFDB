using Microsoft.Extensions.Logging;
using R5.FFDB.Components.PlayerData;
using R5.FFDB.Components.Roster;
using R5.FFDB.Components.WeekStats;
using R5.FFDB.Engine.SourceResolvers;
using System;
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
		

		public void TestLogging()
		{
			_logger.LogTrace("this is a trace log.");
			_logger.LogDebug("this is a debug log");
			_logger.LogCritical("this is a criterial log");
			
			try
			{
				Throw();
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "caught an error", new object[] { "something", 23, true });
			}
			
		}

		private void Throw()
		{
			throw new InvalidOperationException("an invalid op exception was thrown!");
		}
	}

	public interface ISourcesFactory
	{
		Task<Sources> GetAsync();
	}

	public class SourcesFactory : ISourcesFactory
	{
		private IPlayerDataSourceResolver _playerDataSourceResolver { get; }
		private IRosterSourceResolver _rosterSourceResolver { get; }
		private IWeekStatsSourceResolver _weekStatsSourceResolver { get; }

		public SourcesFactory(
			IPlayerDataSourceResolver playerDataSourceResolver,
			IRosterSourceResolver rosterSourceResolver,
			IWeekStatsSourceResolver weekStatsSourceResolver)
		{
			_playerDataSourceResolver = playerDataSourceResolver;
			_rosterSourceResolver = rosterSourceResolver;
			_weekStatsSourceResolver = weekStatsSourceResolver;
		}

		public async Task<Sources> GetAsync()
		{
			var playerData = await _playerDataSourceResolver.GetAsync();
			var roster = await _rosterSourceResolver.GetAsync();
			var weekStats = await _weekStatsSourceResolver.GetAsync();

			return new Sources
			{
				PlayerData = playerData,
				Roster = roster,
				WeekStats = weekStats
			};
		}
	}

	public class Sources
	{
		public IPlayerDataSource PlayerData { get; set; }
		public IRosterSource Roster { get; set; }
		public IWeekStatsSource WeekStats { get; set; }
	}
	
}
