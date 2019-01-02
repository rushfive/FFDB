using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using R5.FFDB.Components.CoreData.PlayerProfile;
using R5.FFDB.Components.CoreData.TeamData.Models;
using R5.FFDB.Components.CoreData.TeamGameHistory.Values;
using R5.FFDB.Components.Resolvers;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// Reading and parsing the stats json files into JObjects takes a long time.
// These files are required for 2 different data maps (playerWeekTeam and teamWeekStats)
// so we'll parse the files just one time, building both maps simultaneously.

namespace R5.FFDB.Components.CoreData.TeamGameHistory
{
	public interface IGameStatsParser
	{
		void ParseFilesToMapValues();
	}

	public class GameStatsParser : IGameStatsParser
	{
		private static List<string> _statKeys = new List<string>
		{
			"passing", "rushing", "receiving", "fumbles", "kicking", "punting", "kickret", "puntret", "defense"
		};

		private ILogger<GameStatsParser> _logger { get; }
		private DataDirectoryPath _dataPath { get; }
		private GameWeekMapValue _gameWeekMap { get; }
		private IPlayerIdMapper _playerIdMapper { get; }
		private PlayerWeekTeamMapValue _playerWeekTeamMap { get; }
		private TeamWeekStatsMapValue _teamWeekStatsMap { get; }

		public GameStatsParser(
			ILogger<GameStatsParser> logger,
			DataDirectoryPath dataPath,
			GameWeekMapValue gameWeekMap,
			IPlayerIdMapper playerIdMapper,
			PlayerWeekTeamMapValue playerWeekTeamMap,
			TeamWeekStatsMapValue teamWeekStatsMap)
		{
			_logger = logger;
			_dataPath = dataPath;
			_gameWeekMap = gameWeekMap;
			_playerIdMapper = playerIdMapper;
			_playerWeekTeamMap = playerWeekTeamMap;
			_teamWeekStatsMap = teamWeekStatsMap;
		}

		public void ParseFilesToMapValues()
		{
			_logger.LogInformation("Parsing game stat files and building into maps. This could take a minute..");

			var playerWeekTeamMap = new Dictionary<string, Dictionary<WeekInfo, int>>();
			var teamWeekStatsMap = new Dictionary<WeekInfo, Dictionary<int, TeamWeekStats>>();

			List<string> gameStatFiles = DirectoryFilesResolver.GetFileNames(
				_dataPath.Static.TeamGameHistoryGameStats,
				excludeExtensions: true);

			Dictionary<string, WeekInfo> gameWeekMap = _gameWeekMap.Get();

			foreach (string gameId in gameStatFiles)
			{
				WeekInfo week = gameWeekMap[gameId];
				JObject json = JObject.Parse(File.ReadAllText(_dataPath.Static.TeamGameHistoryGameStats + $"{gameId}.json"));

				AddFromGame(gameId, week, json, playerWeekTeamMap, teamWeekStatsMap);
			}

			_playerWeekTeamMap.Set(playerWeekTeamMap);
			_teamWeekStatsMap.Set(teamWeekStatsMap);

			_logger.LogInformation("Finished parsing game stat files into maps.");
		}

		private void AddFromGame(string gameId, WeekInfo week, JObject json,
			Dictionary<string, Dictionary<WeekInfo, int>> playerWeekTeamMap,
			Dictionary<WeekInfo, Dictionary<int, TeamWeekStats>> teamWeekStatsMap)
		{
			AddForTeam("home", gameId, json, week, playerWeekTeamMap, teamWeekStatsMap);
			AddForTeam("away", gameId, json, week, playerWeekTeamMap, teamWeekStatsMap);
		}

		private void AddForTeam(string teamType, string gameId, JObject fileJson, WeekInfo week, 
			Dictionary<string, Dictionary<WeekInfo, int>> playerWeekTeamMap,
			Dictionary<WeekInfo, Dictionary<int, TeamWeekStats>> teamWeekStatsMap)
		{
			AddForPlayerWeekTeamMap(teamType, gameId, fileJson, week, playerWeekTeamMap);
			AddForTeamWeekStatsMap(teamType, gameId, fileJson, week, teamWeekStatsMap);
		}

		private void AddForPlayerWeekTeamMap(string teamType, string gameId, JObject fileJson, 
			WeekInfo week, Dictionary<string, Dictionary<WeekInfo, int>> map)
		{
			int teamId = TeamDataStore.GetIdFromAbbreviation(
				(string)fileJson.SelectToken($"{gameId}.{teamType}.abbr"),
				includePriorLookup: true);

			foreach (string statKey in _statKeys)
			{
				if (!fileJson.SelectToken($"{gameId}.{teamType}.stats").TryGetToken(statKey, out JToken stats))
				{
					continue;
				}

				foreach (string gsis in stats.ChildPropertyNames())
				{
					if (!_playerIdMapper.TryGetNflFromGsis(gsis, out string nflId))
					{
						// most likely insignificant players that we dont care about
						continue;
					}

					if (!map.ContainsKey(nflId))
					{
						map[nflId] = new Dictionary<WeekInfo, int>();
					}

					map[nflId][week] = teamId;
				}
			}
		}

		private void AddForTeamWeekStatsMap(string teamType, string gameId, JObject fileJson,
			WeekInfo week, Dictionary<WeekInfo, Dictionary<int, TeamWeekStats>> map)
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
