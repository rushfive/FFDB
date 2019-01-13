﻿using Newtonsoft.Json;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Components.CoreData.WeekStats.Models
{
	// model for req payload seen in this example:
	// http://api.fantasy.nfl.com/v2/players/weekstats?season=2018&week=7

	// The response model is confusing. Deeply nested object containing bunch of objects and maps/dicts.

	// Used this: https://app.quicktype.io/#l=cs
	// to get an idea of how to deserialize into types. copy pasting the entire response didnt work, so i 
	// grabbed only the "Games" section (which is all we need i think)
	public class WeekStatsJson
	{
		// the resp only seems to contain a single game id (kvp), prob representing the requested weeks game
		[JsonProperty("games")]
		public Dictionary<string, WeekStatsGameJson> Games { get; set; }

		public static Core.Entities.WeekStats ToCoreEntity(WeekStatsJson model, WeekInfo week,
			Func<string, int?> weekTeamResolver)
		{
			var players = new List<Core.Entities.PlayerWeekStats>();

			WeekStatsGameJson games = model.Games.Single().Value;
			foreach (KeyValuePair<string, WeekStatsPlayerJson> player in games.Players)
			{
				Dictionary<string, string> modelStats = player.Value.Stats.WeekStats.Single().Value.Single().Value;

				if (!modelStats.ContainsKey("pts") || modelStats["pts"] == null)
				{
					// player didn't play this week
					continue;
				}

				var stats = new Dictionary<Core.Models.WeekStatType, double>();

				foreach (KeyValuePair<string, string> stat in modelStats)
				{
					if (stat.Key == "pts")
					{
						continue;
					}

					if (!string.IsNullOrWhiteSpace(stat.Value) &&
						double.TryParse(stat.Value, out double value))
					{
						int key = int.Parse(stat.Key);

						if (!Enum.IsDefined(typeof(Core.Models.WeekStatType), key))
						{
							continue;
						}

						stats.Add((Core.Models.WeekStatType)key, value);
					}
				}

				string nflId = player.Key;
				int? teamId = weekTeamResolver(nflId);

				players.Add(new Core.Entities.PlayerWeekStats
				{
					NflId = nflId,
					Stats = stats,
					TeamId = teamId
				});
			}

			return new Core.Entities.WeekStats
			{
				Week = week,
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
