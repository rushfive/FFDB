using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using R5.FFDB.Components.CoreData.TeamData.Models;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace R5.FFDB.Components.CoreData.TeamGameHistory
{
	public interface ITeamGameStatsService
	{
		List<TeamWeekStats> GetForWeek(WeekInfo week);
		//List<TeamWeekStats> Get();
	}

	public class TeamGameStatsService : ITeamGameStatsService
	{
		private ILogger<TeamGameStatsService> _logger { get; }
		private DataDirectoryPath _dataPath { get; }

		public TeamGameStatsService(
			ILogger<TeamGameStatsService> logger,
			DataDirectoryPath dataPath)
		{
			_logger = logger;
			_dataPath = dataPath;
		}

		public List<TeamWeekStats> GetForWeek(WeekInfo week)
		{
			var result = new List<TeamWeekStats>();

			List<string> gameIds = GetGameIds(week);
			foreach (var gameId in gameIds)
			{
				JObject json = JObject.Parse(File.ReadAllText(_dataPath.Static.TeamGameHistoryGameStats + $"{gameId}.json"));

				AddForTeam("home", gameId, json, week, result);
				AddForTeam("away", gameId, json, week, result);
			}

			return result;
		}

		private List<string> GetGameIds(WeekInfo week)
		{
			var result = new List<string>();

			var filePath = _dataPath.Static.TeamGameHistoryWeekGames + $"{week.Season}-{week.Week}.xml";

			XElement weekGameXml = XElement.Load(filePath);

			XElement gameNode = weekGameXml.Elements("gms").Single();

			foreach (XElement game in gameNode.Elements("g"))
			{
				string gameId = game.Attribute("eid").Value;
				result.Add(gameId);
			}

			return result;
		}

		private void AddForTeam(string teamType, string gameId, JObject json,
			WeekInfo week, List<TeamWeekStats> result)
		{
			int teamId = TeamDataStore.GetIdFromAbbreviation(
				(string)json.SelectToken($"{gameId}.{teamType}.abbr"),
				includePriorLookup: true);

			var stats = new TeamWeekStats(teamId, teamType == "home", week);

			JToken score = json.SelectToken($"{gameId}.{teamType}.score");
			if (score == null)
			{
				throw new InvalidOperationException($"Failed to parse score object for {teamType} team in game '{gameId}'.");
			}

			stats.SetPointsScored(score);

			JToken teamStats = json.SelectToken($"{gameId}.home.stats.team");
			if (teamStats == null)
			{
				throw new InvalidOperationException($"Failed to parse team stats object for {teamType} team in game '{gameId}'.");
			}

			stats.SetTeamStats(teamStats);

			result.Add(stats);
		}

		// pre per-week below

		//public List<TeamWeekStats> Get()
		//{
		//	var result = new List<TeamWeekStats>();

		//	Dictionary<WeekInfo, Dictionary<int, TeamWeekStats>> map = _teamWeekStatsMap.Get();
		//	foreach (Dictionary<int, TeamWeekStats> innerMap in map.Values)
		//	{
		//		result.AddRange(innerMap.Values);
		//	}

		//	return result;
		//}
	}
}
