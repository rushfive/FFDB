using Newtonsoft.Json;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData.Static.WeekGameMap.Sources.V1.Models
{
	public class WeekGamesVersionedModel
	{
		[JsonProperty("week")]
		public WeekInfo Week { get; set; }

		[JsonProperty("games")]
		public List<Game> Games { get; set; }

		public class Game
		{
			[JsonProperty("nflGameId")]
			public string NflGameId { get; set; }

			[JsonProperty("gsisGameId")]
			public string GsisGameId { get; set; }

			[JsonProperty("homeTeamId")]
			public int HomeTeamId { get; set; }

			[JsonProperty("awayTeamId")]
			public int AwayTeamId { get; set; }
		}
	}
}
