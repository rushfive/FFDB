using Newtonsoft.Json.Linq;
using R5.FFDB.Components.CoreData.Static.Players;
using R5.FFDB.Components.CoreData.Static.TeamStats.Sources.V1.Models;
using R5.FFDB.Core;
using R5.FFDB.Core.Models;
using R5.Lib.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.TeamStats.Sources.V1.Mappers
{
	public interface IToVersionedMapper : IAsyncMapper<string, TeamWeekStatsVersioned, (string gameId, WeekInfo week)> { }

	public class ToVersionedMapper : IToVersionedMapper
	{
		private IPlayerIdMappings _playerIdMappings { get; }

		public ToVersionedMapper(IPlayerIdMappings playerIdMappings)
		{
			_playerIdMappings = playerIdMappings;
		}

		private static List<string> _statKeys = new List<string>
		{
			"passing", "rushing", "receiving", "fumbles", "kicking", "punting", "kickret", "puntret", "defense"
		};

		public async Task<TeamWeekStatsVersioned> MapAsync(string httpResponse, (string, WeekInfo) gameWeek)
		{
			JObject json = JObject.Parse(httpResponse);

			Dictionary<string, string> gsisNflIdMap = await _playerIdMappings.GetGsisToNflMapAsync();

			var homeStats = GetStats(json, gameWeek.Item1, "home", gsisNflIdMap);
			var awayStats = GetStats(json, gameWeek.Item1, "away", gsisNflIdMap);

			return new TeamWeekStatsVersioned
			{
				Week = gameWeek.Item2,
				HomeTeamStats = homeStats,
				AwayTeamStats = awayStats
			};
		}

		private static TeamWeekStatsVersioned.Stats GetStats(JObject json, 
			string gameId, string teamType, Dictionary<string, string> gsisNflIdMap)
		{
			Debug.Assert(teamType == "home" || teamType == "away");

			int teamId = TeamDataStore.GetIdFromAbbreviation(
				(string)json.SelectToken($"{gameId}.{teamType}.abbr"),
				includePriorLookup: true);

			List<string> nflIds = GetPlayerGsisIds(json, gameId, teamType)
				.Where(gsis => gsisNflIdMap.ContainsKey(gsis))
				.Select(gsis => gsisNflIdMap[gsis])
				.ToList();

			var stats = new TeamWeekStatsVersioned.Stats
			{
				Id = teamId,
				PlayerNflIds = nflIds
			};

			SetPointsScored(stats, json, gameId, teamType);
			SetTeamStats(stats, json, gameId, teamType);

			return stats;
		}

		private static List<string> GetPlayerGsisIds(JObject json, string gameId, string teamType)
		{
			var result = new List<string>();

			foreach (string statKey in _statKeys)
			{
				if (!json.SelectToken($"{gameId}.{teamType}.stats").TryGetToken(statKey, out JToken stats))
				{
					continue;
				}

				List<string> gsisIds = stats.ChildPropertyNames();
				result.AddRange(gsisIds);
			}

			return result;
		}

		private static void SetPointsScored(TeamWeekStatsVersioned.Stats stats, JObject json,
			string gameId, string teamType)
		{
			JToken score = json.SelectToken($"{gameId}.{teamType}.score");
			if (score == null)
			{
				throw new InvalidOperationException($"Failed to parse score object for {teamType} team in game '{gameId}'.");
			}

			stats.PointsFirstQuarter = (int)score["1"];
			stats.PointsSecondQuarter = (int)score["2"];
			stats.PointsThirdQuarter = (int)score["3"];
			stats.PointsFourthQuarter = (int)score["4"];
			stats.PointsOverTime = (int)score["5"];
			stats.PointsTotal = (int)score["T"];
		}

		private static void SetTeamStats(TeamWeekStatsVersioned.Stats stats, JObject json,
			string gameId, string teamType)
		{
			JToken teamStats = json.SelectToken($"{gameId}.{teamType}.stats.team");
			if (teamStats == null)
			{
				throw new InvalidOperationException($"Failed to parse team stats object for {teamType} team in game '{gameId}'.");
			}

			stats.FirstDowns = (int)teamStats["totfd"];
			stats.TotalYards = (int)teamStats["totyds"];
			stats.PassingYards = (int)teamStats["pyds"];
			stats.RushingYards = (int)teamStats["ryds"];
			stats.Penalties = (int)teamStats["pen"];
			stats.PenaltyYards = (int)teamStats["penyds"];
			stats.Turnovers = (int)teamStats["trnovr"];
			stats.Punts = (int)teamStats["pt"];
			stats.PuntYards = (int)teamStats["ptyds"];

			string timeOfPosession = (string)teamStats["top"];
			var split = timeOfPosession.Split(':');
			stats.TimeOfPossessionSeconds = int.Parse(split[0]) * 60 + int.Parse(split[1]);
		}
	}
}
