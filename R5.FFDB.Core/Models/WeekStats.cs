using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Models
{
	public class WeekStats
	{
		public List<PlayerStats> Players { get; set; }
	}

	public class PlayerStats
	{
		public string NflId { get; set; }
		public Dictionary<WeekStatType, double> Stats { get; set; }

		// players dont have values for all stats 
		public double GetValueFor(WeekStatType type)
		{
			return Stats.ContainsKey(type) ? Stats[type] : 0;
		}
	}
}
