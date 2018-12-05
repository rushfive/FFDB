using R5.FFDB.Components.PlayerProfile;
using R5.FFDB.Components.PlayerTeamHistory;
using R5.FFDB.Components.Roster;
using R5.FFDB.Components.WeekStats;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Engine.Source
{
	public class Sources
	{
		public IPlayerProfileSource PlayerProfile { get; set; }
		public IRosterSource Roster { get; set; }
		public IWeekStatsSource WeekStats { get; set; }
		public IPlayerTeamHistorySource PlayerTeamHistory { get; set; }
	}
}
