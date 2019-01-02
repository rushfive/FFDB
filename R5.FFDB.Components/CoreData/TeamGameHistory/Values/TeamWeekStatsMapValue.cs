using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData.TeamGameHistory.Values
{
	public class TeamWeekStatsMapValue : ValueProvider<Dictionary<WeekInfo, Dictionary<int, TeamWeekStats>>>
	{
		public TeamWeekStatsMapValue()
			: base("Team Week Stats Map")
		{
		}

		protected override Dictionary<WeekInfo, Dictionary<int, TeamWeekStats>> ResolveValue()
		{
			throw new InvalidOperationException($"Value must be resolved by the '{nameof(GameStatsParser)}'.");
		}
	}
}
