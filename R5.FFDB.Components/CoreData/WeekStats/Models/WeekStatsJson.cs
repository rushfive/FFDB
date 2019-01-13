using Newtonsoft.Json;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Components.CoreData.WeekStats.Models
{
	public class WeekStatsJson
	{
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

						if (!Enum.IsDefined(typeof(WeekStatType), key))
						{
							continue;
						}

						stats.Add((WeekStatType)key, value);
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
