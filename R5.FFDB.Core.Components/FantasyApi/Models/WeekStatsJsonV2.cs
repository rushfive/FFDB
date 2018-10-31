using Newtonsoft.Json;
using R5.FFDB.Core.Abstractions;
using R5.FFDB.Core.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Core.Components.FantasyApi.Models
{
	// model for req payload seen in this example:
	// http://api.fantasy.nfl.com/v2/players/weekstats?season=2018&week=7

	// The response model is confusing. Deeply nested object containing bunch of objects and maps/dicts.

	// Used this: https://app.quicktype.io/#l=cs
	// to get an idea of how to deserialize into types. copy pasting the entire response didnt work, so i 
	// grabbed only the "Games" section (which is all we need i think)

	public class WeekStatsJsonV2
	{
		// the resp only seems to contain a single game id (kvp), prob representing the requested weeks game
		[JsonProperty("games")]
		public Dictionary<string, WeekStatsGameJson> Games { get; set; }

		public static WeekStats ToCoreEntity(WeekStatsJsonV2 model)
		{
			var players = new List<PlayerStats>();

			WeekStatsGameJson games = model.Games.Single().Value;
			foreach(KeyValuePair<string, WeekStatsPlayerJson> player in games.Players)
			{
				Dictionary<string, string> modelStats = player.Value.Stats.WeekStats.Single().Value.Single().Value;

				if (!modelStats.ContainsKey("pts") || modelStats["pts"] == null)
				{
					// player didn't play this week
					continue;
				}

				var stats = new Dictionary<WeekStatType, double>();
				
				foreach(KeyValuePair<string, string> stat in modelStats)
				{
					if (stat.Key == "pts")
					{
						continue;
					}

					if (!string.IsNullOrWhiteSpace(stat.Value) &&
						double.TryParse(stat.Value, out double value))
					{
						int key = int.Parse(stat.Key);

						if (!Enum.IsDefined(typeof(WeekStatType), key))
						{
							continue;
						}
						
						stats.Add((WeekStatType)key, value);
					}
				}

				players.Add(new PlayerStats
				{
					NflId = player.Key,
					Stats = stats
				});
			}

			return new WeekStats
			{
				Players = players
			};
		}
	}

	public class WeekStatsGameJson
	{
		[JsonProperty("gameId")]
		public string GameId { get; set; }
		
		[JsonProperty("players")]
		public Dictionary<string, WeekStatsPlayerJson> Players { get; set; }
	}

	public class WeekStatsPlayerJson
	{
		[JsonProperty("stats")]
		public WeekStatsStatsJson Stats { get; set; }
	}

	public class WeekStatsStatsJson
	{
		[JsonProperty("week")]
		// first key = year, eg "2018"
		// second key = week, eg "7"
		// third key = stat type/id and the value is a number (serialized as a string, prob just assume to be double?)
		public Dictionary<string, Dictionary<string, Dictionary<string, string>>> WeekStats { get; set; }
	}
}
