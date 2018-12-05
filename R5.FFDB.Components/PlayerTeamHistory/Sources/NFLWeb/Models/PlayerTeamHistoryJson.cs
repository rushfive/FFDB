using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.PlayerTeamHistory.Sources.NFLWeb.Models
{
	public class PlayerTeamHistoryJson
	{
		[JsonProperty("nflId")]
		public string NflId { get; set; }

		[JsonProperty("seasonWeekTeamMap")]
		public Dictionary<int, Dictionary<int, int>> SeasonWeekTeamMap { get; set; }
	}
}
