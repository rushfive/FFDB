using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.ErrorFileLog.ErrorTypes
{
	public class PlayerTeamHistoryFetchError : ErrorFile
	{
		[JsonProperty("nflId")]
		public string NflId { get; set; }
	}

	public class PlayerTeamHistoryWeekUnavailableError : ErrorFile
	{
		[JsonProperty("nflId")]
		public string NflId { get; set; }

		[JsonProperty("season")]
		public int Season { get; set; }

		[JsonProperty("week")]
		public int Week { get; set; }
	}
}
