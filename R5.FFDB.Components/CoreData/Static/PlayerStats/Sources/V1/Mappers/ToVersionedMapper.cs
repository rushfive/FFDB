using Newtonsoft.Json.Linq;
using R5.FFDB.Components.CoreData.Static.PlayerStats.Sources.V1.Models;
using R5.FFDB.Components.CoreData.Static.TeamStats;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.PlayerStats.Sources.V1.Mappers
{
	public interface IToVersionedMapper : IAsyncMapper<string, PlayerWeekStatsVersioned, WeekInfo> { }

	public class ToVersionedMapper : IToVersionedMapper
	{
		private ITeamStatsCache _teamStatsCache { get; }

		public ToVersionedMapper(ITeamStatsCache teamStatsCache)
		{
			_teamStatsCache = teamStatsCache;
		}

		public async Task<PlayerWeekStatsVersioned> MapAsync(string httpResponse, WeekInfo week)
		{
			JObject playersObject = SourceJsonReader.GetPlayersObject(httpResponse);
			Dictionary<string, int> playerTeamMap = await _teamStatsCache.GetPlayerTeamMapAsync(week);

			var players = GetPlayers(playersObject, week, playerTeamMap);

			return new PlayerWeekStatsVersioned
			{
				Week = week,
				Players = players
			};
		}

		private static List<PlayerWeekStatsVersioned.Player> GetPlayers(JObject playersObject, 
			WeekInfo week, Dictionary<string, int> playerTeamMap)
		{
			var result = new List<PlayerWeekStatsVersioned.Player>();

			foreach(var p in playersObject)
			{
				string nflId = p.Key;

				var statsObject = SourceJsonReader.GetStatsObjectForPlayer(p.Value, week);
				Dictionary<WeekStatType, double> stats = SourceJsonReader.ResolveStatsMapFromObject(statsObject);

				var player = new PlayerWeekStatsVersioned.Player
				{
					NflId = nflId,
					Stats = stats
				};

				if (playerTeamMap.TryGetValue(nflId, out int teamId))
				{
					player.TeamId = teamId;
				}

				result.Add(player);
			}

			return result;
		}
	}
}
