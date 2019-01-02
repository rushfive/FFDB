using R5.FFDB.Components.CoreData;
using R5.FFDB.Components.CoreData.PlayerProfile;
using R5.FFDB.Components.CoreData.Roster;
using R5.FFDB.Components.CoreData.TeamGameHistory;
using R5.FFDB.Components.CoreData.WeekStats;
using R5.FFDB.Components.ValueProviders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Engine.Source
{
	public class CoreDataSourcesResolver : AsyncValueProvider<CoreDataSources>
	{
		private IPlayerProfileSource _playerProfile { get; }
		private IRosterSource _roster { get; }
		private IWeekStatsSource _weekStats { get; }
		private ITeamGameHistorySource _teamGameHistory { get; }

		public CoreDataSourcesResolver(
			IPlayerProfileSource playerProfile,
			IRosterSource roster,
			IWeekStatsSource weekStats,
			ITeamGameHistorySource teamGameHistory)
			: base("Sources")
		{
			_playerProfile = playerProfile;
			_roster = roster;
			_weekStats = weekStats;
			_teamGameHistory = teamGameHistory;
		}

		protected override async Task<CoreDataSources> ResolveValueAsync()
		{
			await CheckSourcesHealthyAsync();

			return new CoreDataSources
			{
				PlayerProfile = _playerProfile,
				Roster = _roster,
				WeekStats = _weekStats,
				TeamGameHistory = _teamGameHistory
			};
		}

		private async Task CheckSourcesHealthyAsync()
		{
			var allSources = new List<ICoreDataSource>
			{
				_playerProfile, _roster, _weekStats, _teamGameHistory
			};

			foreach (ICoreDataSource source in allSources)
			{
				try
				{
					await source.CheckHealthAsync();
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException($"Failed to resolve source '{source.Label}', "
						+ "didn't pass its health check.", ex);
				}
			}
		}
	}
}
