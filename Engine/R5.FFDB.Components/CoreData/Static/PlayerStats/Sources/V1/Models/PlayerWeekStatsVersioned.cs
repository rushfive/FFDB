using Newtonsoft.Json;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData.Static.PlayerStats.Sources.V1.Models
{
	public class PlayerWeekStatsVersioned
	{
		[JsonProperty("week")]
		public WeekInfo Week { get; set; }

		[JsonProperty("players")]
		public List<Player> Players { get; set; }

		public class Player
		{
			[JsonProperty("nflId")]
			public string NflId { get; set; }

			[JsonProperty("stats")]
			public Dictionary<WeekStatType, double> Stats { get; set; }

			[JsonProperty("teamId")]
			public int? TeamId { get; set; }
		}
	}
}
