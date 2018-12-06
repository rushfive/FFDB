using R5.FFDB.Components.PlayerTeamHistory.Sources.NFLWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Components.Stores
{
	public class PlayerTeamHistoryStore
	{
		private Dictionary<string, Dictionary<int, Dictionary<int, int>>> _playerHistoryMap { get; }

		public PlayerTeamHistoryStore(List<PlayerTeamHistoryJson> histories)
		{
			_playerHistoryMap = histories.ToDictionary(h => h.NflId, h => h.SeasonWeekTeamMap);
		}

		public int GetTeam(string nflId, int season, int week)
		{
			if (!_playerHistoryMap.TryGetValue(nflId, out Dictionary<int, Dictionary<int, int>> seasonMap))
			{
				throw new InvalidOperationException($"'{nflId}' is not a valid NFL player id.");
			}

			if (!seasonMap.TryGetValue(season, out Dictionary<int, int> weekMap))
			{
				throw new InvalidOperationException($"Failed to find team history for '{nflId}' in season '{season}'.");
			}

			if (!weekMap.TryGetValue(week, out int teamId))
			{
				throw new InvalidOperationException($"Failed to find team history for '{nflId}' in season '{season}' week '{week}'.");
			}

			return teamId;
		}
	}
}
