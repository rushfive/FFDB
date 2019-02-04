using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using R5.FFDB.Core;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace R5.FFDB.Components.CoreData.TeamGames
{
	[Obsolete("Use new TeamGameDataCache instead")]
	public interface ITeamGameStatsService
	{
		List<TeamWeekStats> GetForWeek(WeekInfo week);
	}
	[Obsolete("Use new TeamGameDataCache instead")]
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

			List<string> gameIds = TeamGamesUtil.GetGameIdsForWeek(week, _dataPath);
			foreach (var gameId in gameIds)
			{
				JObject json = JObject.Parse(File.ReadAllText(_dataPath.Static.TeamGameStats + $"{gameId}.json"));

				AddForTeam("home", gameId, json, week, result);
				AddForTeam("away", gameId, json, week, result);
			}

			return result;
		}

		private void AddForTeam(string teamType, string gameId, JObject json,
			WeekInfo week, List<TeamWeekStats> result)
		{
			int teamId = TeamDataStore.GetIdFromAbbreviation(
				(string)json.SelectToken($"{gameId}.{teamType}.abbr"),
				includePriorLookup: true);

			var stats = new TeamWeekStats
			{
				TeamId = teamId,
				Week = week
			};

			TeamGamesUtil.SetTeamWeekStats(stats, json, gameId, teamType);

			result.Add(stats);
		}
	}
}
