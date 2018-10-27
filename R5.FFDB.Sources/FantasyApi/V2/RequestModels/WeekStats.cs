using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Sources.FantasyApi.V2.RequestModels
{
	// model for req payload seen in this example:
	// http://api.fantasy.nfl.com/v2/players/weekstats?season=2018&week=7

	// The response model is confusing. Deeply nested object containing bunch of objects and maps/dicts.

	// Used this: https://app.quicktype.io/#l=cs
	// to get an idea of how to deserialize into types. copy pasting the entire response didnt work, so i 
	// grabbed only the "Games" section (which is all we need i think)

	public class WeekStats
	{
		// the resp only seems to contain a single game id (kvp), prob representing the requested weeks game
		[JsonProperty("games")]
		public Dictionary<string, WeekStatsGameInfo> Games { get; set; }
	}

	public class WeekStatsGameInfo
	{
		[JsonProperty("gameId")]
		public string GameId { get; set; }
		
		[JsonProperty("players")]
		public Dictionary<string, Player> Players { get; set; }
	}

	public class Player
	{
		[JsonProperty("stats")]
		public Stats Stats { get; set; }
	}

	public class Stats
	{
		[JsonProperty("week")]
		// first key = year, eg "2018"
		// second key = week, eg "7"
		// third key = stat type/id and the value is a number (serialized as a string, prob just assume to be double?)
		public Dictionary<string, Dictionary<string, Dictionary<string, string>>> WeekStats { get; set; }
	}
}
