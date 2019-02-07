using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using R5.FFDB.Components.Extensions;
using R5.FFDB.Components.Extensions.Methods;
using R5.FFDB.Core;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData.TeamGames.Models
{
	public class WeekTeamMatchupStats
	{
		[JsonProperty("week")]
		public WeekInfo Week { get; set; }

		[JsonProperty("stats")]
		public List<TeamGameMatchupStats> Stats { get; set; } = new List<TeamGameMatchupStats>();
	}


	public class TeamGameMatchupStats
	{
		//public WeekInfo Week { get; set; }
		[JsonProperty("home")]
		public TeamData Home { get; set; }

		[JsonProperty("away")]
		public TeamData Away { get; set; }

		private TeamGameMatchupStats(
			//WeekInfo week,
			TeamData home,
			TeamData away)
		{
			//Week = week;
			Home = home;
			Away = away;
		}

		[JsonConstructor]
		private TeamGameMatchupStats() { }

		// json param should be the entire parsed file, just pass it straight here
		public static TeamGameMatchupStats FromGameStats(JObject json, string gameId,
			WeekInfo week, Dictionary<string, string> gsisNflIdMap)
		{
			var home = TeamData.AsHome(json, gameId, gsisNflIdMap);
			var away = TeamData.AsAway(json, gameId, gsisNflIdMap);
			//return new TeamGameMatchupStats(week, home, away);
			return new TeamGameMatchupStats(home, away);
		}
	}

	public class TeamData
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		// list of players from team active, must map from esb to nflid
		[JsonProperty("playerNflIds")]
		public List<string> PlayerNflIds { get; set; } = new List<string>();


		[JsonProperty("pointsFirstQuarter")]
		public int PointsFirstQuarter { get; set; }

		[JsonProperty("pointsSecondQuarter")]
		public int PointsSecondQuarter { get; set; }

		[JsonProperty("pointsThirdQuarter")]
		public int PointsThirdQuarter { get; set; }

		[JsonProperty("pointsFourthQuarter")]
		public int PointsFourthQuarter { get; set; }

		[JsonProperty("pointsOverTime")]
		public int PointsOverTime { get; set; }

		[JsonProperty("pointsTotal")]
		public int PointsTotal { get; set; }


		[JsonProperty("firstDowns")]
		public int FirstDowns { get; set; }

		[JsonProperty("totalYards")]
		public int TotalYards { get; set; }

		[JsonProperty("passingYards")]
		public int PassingYards { get; set; }

		[JsonProperty("rushingYards")]
		public int RushingYards { get; set; }

		[JsonProperty("penalties")]
		public int Penalties { get; set; }

		[JsonProperty("penaltyYards")]
		public int PenaltyYards { get; set; }

		[JsonProperty("turnovers")]
		public int Turnovers { get; set; }

		[JsonProperty("punts")]
		public int Punts { get; set; }

		[JsonProperty("puntYards")]
		public int PuntYards { get; set; }

		[JsonProperty("puntYardsAverage")]
		public int PuntYardsAverage { get; set; }

		[JsonProperty("timeOfPossessionSeconds")]
		public int TimeOfPossessionSeconds { get; set; }

		private static List<string> _statKeys = new List<string>
		{
			"passing", "rushing", "receiving", "fumbles", "kicking", "punting", "kickret", "puntret", "defense"
		};

		[JsonConstructor]
		private TeamData() { }

		public static TeamData AsHome(JObject json, string gameId, Dictionary<string, string> gsisNflIdMap)
		{
			return new TeamData().ResolveAs(json, gameId, "home", gsisNflIdMap);
		}

		public static TeamData AsAway(JObject json, string gameId, Dictionary<string, string> gsisNflIdMap)
		{
			return new TeamData().ResolveAs(json, gameId, "away", gsisNflIdMap);
		}

		private TeamData ResolveAs(JObject json, string gameId, string teamType, Dictionary<string, string> gsisNflIdMap)
		{
			Id = TeamDataStore.GetIdFromAbbreviation(
				(string)json.SelectToken($"{gameId}.{teamType}.abbr"),
				includePriorLookup: true);

			SetPointsScored(json, gameId, teamType);
			SetTeamStats(json, gameId, teamType);
			SetActivePlayers(json, Id, gameId, teamType, gsisNflIdMap);

			return this;
		}

		private void SetPointsScored(JObject json, string gameId, string teamType)
		{
			JToken score = json.SelectToken($"{gameId}.{teamType}.score");
			if (score == null)
			{
				throw new InvalidOperationException($"Failed to parse score object for {teamType} team in game '{gameId}'.");
			}

			PointsFirstQuarter = (int)score["1"];
			PointsSecondQuarter = (int)score["2"];
			PointsThirdQuarter = (int)score["3"];
			PointsFourthQuarter = (int)score["4"];
			PointsOverTime = (int)score["5"];
			PointsTotal = (int)score["T"];
		}

		private void SetTeamStats(JObject json, string gameId, string teamType)
		{
			JToken teamStats = json.SelectToken($"{gameId}.{teamType}.stats.team");
			if (teamStats == null)
			{
				throw new InvalidOperationException($"Failed to parse team stats object for {teamType} team in game '{gameId}'.");
			}

			FirstDowns = (int)teamStats["totfd"];
			TotalYards = (int)teamStats["totyds"];
			PassingYards = (int)teamStats["pyds"];
			RushingYards = (int)teamStats["ryds"];
			Penalties = (int)teamStats["pen"];
			PenaltyYards = (int)teamStats["penyds"];
			Turnovers = (int)teamStats["trnovr"];
			Punts = (int)teamStats["pt"];
			PuntYards = (int)teamStats["ptyds"];
			PuntYardsAverage = (int)teamStats["ptavg"];

			string timeOfPosession = (string)teamStats["top"];
			var split = timeOfPosession.Split(':');
			TimeOfPossessionSeconds = int.Parse(split[0]) * 60 + int.Parse(split[1]);
		}

		private void SetActivePlayers(JObject json, int teamId, string gameId, string teamType, Dictionary<string, string> gsisNflIdMap)
		{
			foreach (string statKey in _statKeys)
			{
				if (!json.SelectToken($"{gameId}.{teamType}.stats").TryGetToken(statKey, out JToken stats))
				{
					continue;
				}

				foreach (string gsis in stats.ChildPropertyNames())
				{
					if (!gsisNflIdMap.TryGetValue(gsis, out string nflId))
					{
						// most likely insignificant players that we dont care about
						continue;
					}

					PlayerNflIds.Add(nflId);
				}
			}
		}
	}
}
