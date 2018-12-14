using R5.FFDB.Engine.Source.Resolvers;
using System.Threading.Tasks;

namespace R5.FFDB.Engine.Source
{
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
				PlayerProfile = playerData,
				Roster = roster,
				WeekStats = weekStats
			};
		}
	}
}
