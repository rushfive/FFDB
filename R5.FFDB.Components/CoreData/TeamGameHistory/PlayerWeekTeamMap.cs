using R5.FFDB.Components.CoreData.TeamGameHistory.Values;
using R5.FFDB.Core.Models;
using System.Collections.Generic;

namespace R5.FFDB.Components.CoreData.TeamGameHistory
{
	// TODO: OLD?
	public interface IPlayerWeekTeamMap
	{
		int? GetTeam(string nflId, WeekInfo week);
	}
	
	public class PlayerWeekTeamMap : IPlayerWeekTeamMap
	{
		private PlayerWeekTeamMapValue _map { get; }

		public PlayerWeekTeamMap(PlayerWeekTeamMapValue map)
		{
			_map = map;
		}

		public int? GetTeam(string nflId, WeekInfo week)
		{
			Dictionary<string, Dictionary<WeekInfo, int>> map = _map.Get();

			if (!map.TryGetValue(nflId, out Dictionary<WeekInfo, int> weekMap)
				|| !weekMap.TryGetValue(week, out int id))
			{
				return null;
			}

			return id;
		}
	}
}
