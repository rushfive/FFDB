using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;

namespace R5.FFDB.Components.CoreData.TeamGameHistory.Values
{
	// Value: Map of player's NFL Id to map of Season+Week to Team Id
	public class PlayerWeekTeamMapValue : ValueProvider<Dictionary<string, Dictionary<WeekInfo, int>>>
	{
		public PlayerWeekTeamMapValue()
			: base("Player Week Team Map")
		{
		}

		protected override Dictionary<string, Dictionary<WeekInfo, int>> ResolveValue()
		{
			throw new InvalidOperationException($"Value must be resolved by the '{nameof(GameStatsParser)}'.");
		}
	}
}
