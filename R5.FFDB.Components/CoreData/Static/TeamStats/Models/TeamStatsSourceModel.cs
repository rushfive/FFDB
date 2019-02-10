using R5.FFDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData.Static.TeamStats.Models
{
	public class TeamStatsSourceModel
	{
		public TeamWeekStats HomeTeamStats { get; set; }
		public TeamWeekStats AwayTeamStats { get; set; }
	}
}
