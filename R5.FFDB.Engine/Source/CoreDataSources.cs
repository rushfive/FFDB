using R5.FFDB.Components.CoreData.PlayerProfile;
using R5.FFDB.Components.CoreData.Roster;
using R5.FFDB.Components.CoreData.TeamGameHistory;
using R5.FFDB.Components.CoreData.WeekStats;

namespace R5.FFDB.Engine.Source
{
	public class CoreDataSources
	{
		public IPlayerProfileSource PlayerProfile { get; set; }
		public IRosterSource Roster { get; set; }
		public IWeekStatsSource WeekStats { get; set; }
		public ITeamGameHistorySource TeamGameHistory { get; set; }
	}
}
