using R5.FFDB.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Game
{
	public class PlayerWeekStats
	{
		// NFL's ID
		// eg Matt Schaub is 2505982
		// http://www.nfl.com/player/mattschaub/2505982/profile
		public string Id { get; set; }
		public Dictionary<WeekStatType, double> Stats { get; }

		public PlayerWeekStats(string id)
		{
			Id = id;
			Stats = new Dictionary<WeekStatType, double>();
		}

		public void ResolveFromWeekStats()
		{

		}
	}
}
