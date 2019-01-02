using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using R5.FFDB.Components.CoreData.PlayerProfile;
using R5.FFDB.Components.CoreData.TeamData.Models;
using R5.FFDB.Components.Resolvers;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace R5.FFDB.Components.CoreData.TeamGameHistory.Values
{
	// Value: Map of player's NFL Id to map of Season+Week to Team Id
	public class PlayerWeekTeamMapValue : ValueProvider<Dictionary<string, Dictionary<WeekInfo, int>>>
	{
		private static List<string> _statKeys = new List<string>
		{
			"passing", "rushing", "receiving", "fumbles", "kicking", "punting", "kickret", "puntret", "defense"
		};
		
		private IPlayerIdMapper _playerIdMapper { get; }
		private ILogger<PlayerWeekTeamMapValue> _logger { get; }
		private GameStatsFilesValue _gameStatsFiles { get; }

		public PlayerWeekTeamMapValue(
			IPlayerIdMapper playerIdMapper,
			ILogger<PlayerWeekTeamMapValue> logger,
			GameStatsFilesValue gameStatsFiles)
			: base("Player Week Team Map")
		{
			_playerIdMapper = playerIdMapper;
			_logger = logger;
			_gameStatsFiles = gameStatsFiles;
		}

		protected override Dictionary<string, Dictionary<WeekInfo, int>> ResolveValue()
		{
			_logger.LogDebug("Resolving player week team mappings.");

			var result = new Dictionary<string, Dictionary<WeekInfo, int>>();

			List<(string, WeekInfo, JObject)> gameStats = _gameStatsFiles.Get();
			foreach((string id, WeekInfo week, JObject json) in gameStats)
			{
				AddFromGame(id, week, json, result);
			}

			_logger.LogDebug("Finished resolving player week team mappings.");
			return result;
		}

		private void AddFromGame(string gameId, WeekInfo week, JObject json, Dictionary<string, Dictionary<WeekInfo, int>> map)
		{
			AddForTeam("home", gameId, json, week, map);
			AddForTeam("away", gameId, json, week, map);
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
	}
}
