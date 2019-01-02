using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using R5.FFDB.Components.CoreData.TeamData.Models;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData.TeamGameHistory.Values
{
	public class TeamWeekStatsMapValue : ValueProvider<Dictionary<WeekInfo, Dictionary<int, TeamWeekStats>>>
	{
		private ILogger<TeamWeekStatsMapValue> _logger { get; }
		private GameStatsFilesValue _gameStatsFiles { get; }

		public TeamWeekStatsMapValue(
			ILogger<TeamWeekStatsMapValue> logger,
			GameStatsFilesValue gameStatsFiles)
			: base("Team Week Stats Map")
		{
			_logger = logger;
			_gameStatsFiles = gameStatsFiles;
		}

		protected override Dictionary<WeekInfo, Dictionary<int, TeamWeekStats>> ResolveValue()
		{
			_logger.LogDebug("Resolving team week stats.");

			var result = new Dictionary<WeekInfo, Dictionary<int, TeamWeekStats>>();

			List<(string, WeekInfo, JObject)> gameStats = _gameStatsFiles.Get();
			foreach ((string id, WeekInfo week, JObject json) in gameStats)
			{
				AddFromGame(id, week, json, result);
			}

			_logger.LogDebug("Finished resolving team week stats.");
			return result;
		}

		private void AddFromGame(string gameId, WeekInfo week, JObject json, 
			Dictionary<WeekInfo, Dictionary<int, TeamWeekStats>> map)
		{
			AddForTeam("home", gameId, json, week, map);
			AddForTeam("away", gameId, json, week, map);
		}

		private void AddForTeam(string teamType, string gameId, JObject fileJson, WeekInfo week,
			Dictionary<WeekInfo, Dictionary<int, TeamWeekStats>> map)
		{
			int teamId = TeamDataStore.GetIdFromAbbreviation(
				(string)fileJson.SelectToken($"{gameId}.{teamType}.abbr"),
				includePriorLookup: true);

			var stats = new TeamWeekStats(teamId, teamType == "home", week);

			JToken score = fileJson.SelectToken($"{gameId}.{teamType}.score");
			if (score == null)
			{
				throw new InvalidOperationException($"Failed to parse score object for {teamType} team in game '{gameId}'.");
			}

			stats.SetPointsScored(score);

			JToken teamStats = fileJson.SelectToken($"{gameId}.home.stats.team");
			if (teamStats == null)
			{
				throw new InvalidOperationException($"Failed to parse team stats object for {teamType} team in game '{gameId}'.");
			}

			stats.SetTeamStats(teamStats);
			
			if (!map.ContainsKey(week))
			{
				map[week] = new Dictionary<int, TeamWeekStats>();
			}

			map[week][teamId] = stats;
		}
	}
}
