using R5.FFDB.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Request
{
	// Deserialized representation of the response from Fantasy Api
	public class FantasyApiWeekStats
	{
		public Dictionary<string, FantasyApiPlayerStats> Players { get; set; }	
	}

	public class FantasyApiPlayerStats
	{
		public Dictionary<WeekStatType, double> Stats { get; set; }

		public double GetValueFor(WeekStatType type)
		{
			return Stats.ContainsKey(type) ? Stats[type] : 0;
		}
	}
}
