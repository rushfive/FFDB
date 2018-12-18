using Newtonsoft.Json.Linq;
using R5.FFDB.Components.CoreData.TeamData.Models;
using R5.FFDB.Components.Mappers;
using R5.FFDB.Components.Resolvers;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace R5.FFDB.Components.CoreData.TeamGameHistory.Models
{
	// Value: Map of player's NFL Id to map of Season+Week to Team Id
	public class PlayerWeekTeamMap : ValueProvider<Dictionary<string, Dictionary<WeekInfo, int>>>
	{
		private static List<string> _statKeys = new List<string>
		{
			"passing", "rushing", "receiving", "fumbles", "kicking", "punting", "kickret", "puntret", "defense"
		};

		private DataDirectoryPath _dataPath { get; }
		private GameWeekMap _gameWeekMap { get; }
		private IPlayerIdMapper _playerIdMapper { get; }

		public PlayerWeekTeamMap(
			DataDirectoryPath dataPath,
			GameWeekMap gameWeekMap,
			IPlayerIdMapper playerIdMapper)
			: base("Player Week Team Map")
		{
			_dataPath = dataPath;
			_gameWeekMap = gameWeekMap;
			_playerIdMapper = playerIdMapper;
		}

		protected override Dictionary<string, Dictionary<WeekInfo, int>> ResolveValue()
		{
			var result = new Dictionary<string, Dictionary<WeekInfo, int>>();

			Dictionary<string, WeekInfo> gameWeekMap = _gameWeekMap.Get();

			List<string> gameStatFiles = DirectoryFilesResolver.GetFileNames(_dataPath.Static.TeamGameHistoryGameStats, excludeExtensions: true);
			foreach (string gameId in gameStatFiles)
			{
				WeekInfo week = gameWeekMap[gameId];

				AddFromGame(gameId, week, result);
			}

			return result;
		}

		private void AddFromGame(string gameId, WeekInfo week, Dictionary<string, Dictionary<WeekInfo, int>> map)
		{
			JObject fileJson = JObject.Parse(File.ReadAllText(_dataPath.Static.TeamGameHistoryGameStats + $"{gameId}.json"));

			AddForTeam("home", gameId, fileJson, week, map);
			AddForTeam("away", gameId, fileJson, week, map);
		}

		private void AddForTeam(string teamType, string gameId, JObject fileJson, WeekInfo week, Dictionary<string, Dictionary<WeekInfo, int>> map)
		{
			int teamId = TeamDataStore.GetIdFromAbbreviation(
				(string)fileJson.SelectToken($"{gameId}.{teamType}.abbr"),
				includePriorLookup: true);

			foreach(string statKey in _statKeys)
			{
				if (!fileJson.SelectToken($"{gameId}.{teamType}.stats").TryGetToken(statKey, out JToken stats))
				{
					continue;
				}

				foreach(string gsis in stats.ChildPropertyNames())
				{
					if (!_playerIdMapper.TryGetNflFromGsis(gsis, out string nflId))
					{
						// todo: file error log?
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
	}
}
