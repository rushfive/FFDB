using Newtonsoft.Json.Linq;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace R5.FFDB.Components.CoreData.TeamGames
{
	public interface IPlayerWeekTeamResolverFactory
	{
		Task<Func<string, int?>> GetForWeekAsync(WeekInfo week);
	}

	public class PlayerWeekTeamResolverFactory : IPlayerWeekTeamResolverFactory
	{
		private static List<string> _statKeys = new List<string>
		{
			"passing", "rushing", "receiving", "fumbles", "kicking", "punting", "kickret", "puntret", "defense"
		};

		private DataDirectoryPath _dataPath { get; }
		private IDatabaseProvider _dbProvider { get; }

		public PlayerWeekTeamResolverFactory(
			DataDirectoryPath dataPath,
			IDatabaseProvider dbProvider)
		{
			_dataPath = dataPath;
			_dbProvider = dbProvider;
		}

		// Returns a func that takes an nfl id, and returns the teams id
		public async Task<Func<string, int?>> GetForWeekAsync(WeekInfo week)
		{
			var map = new Dictionary<string, int>();
			
			Dictionary<string, string> gsisNflIdMap = await GetGsisToNflIdMapAsync();

			List<string> gameIds = GetGameIds(week);
			foreach (string gameId in gameIds)
			{
				JObject json = JObject.Parse(File.ReadAllText(_dataPath.Static.TeamGameHistoryGameStats + $"{gameId}.json"));

				AddForTeam("home", gameId, json, gsisNflIdMap, map);
				AddForTeam("away", gameId, json, gsisNflIdMap, map);
			}

			return (string nflId) =>
			{
				if (!map.TryGetValue(nflId, out int teamId))
				{
					return null;
				}
				return teamId;
			};
		}

		private async Task<Dictionary<string, string>> GetGsisToNflIdMapAsync()
		{
			IDatabaseContext dbContext = _dbProvider.GetContext();
			List<Player> players = await dbContext.Player.GetAllAsync();
			return players.ToDictionary(p => p.GsisId, p => p.NflId);
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
			Dictionary<string, string> gsisNflIdMap, Dictionary<string, int> map)
		{
			int teamId = TeamDataStore.GetIdFromAbbreviation(
				(string)json.SelectToken($"{gameId}.{teamType}.abbr"),
				includePriorLookup: true);

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

					map[nflId] = teamId;
				}
			}
		}

		
	}
}
