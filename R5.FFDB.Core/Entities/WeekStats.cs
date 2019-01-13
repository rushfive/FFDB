using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Entities
{
	public class WeekStats
	{
		public WeekInfo Week { get; set; }
		public List<PlayerWeekStats> Players { get; set; }
	}

	public class PlayerWeekStats
	{
		public string NflId { get; set; }
		public Dictionary<WeekStatType, double> Stats { get; set; }
		public int? TeamId { get; set; }
	}
}
