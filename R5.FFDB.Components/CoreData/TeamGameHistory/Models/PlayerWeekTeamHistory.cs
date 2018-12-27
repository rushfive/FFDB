using Newtonsoft.Json.Linq;
using R5.FFDB.Components.CoreData.TeamData.Models;
using R5.FFDB.Components.Mappers;
using R5.FFDB.Components.Resolvers;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace R5.FFDB.Components.CoreData.TeamGameHistory.Models
{
	public interface IPlayerWeekTeamHistory
	{
		int? GetTeam(string nflId, WeekInfo week);
		//bool TryGetTeam(string nflId, int season, int week, out int teamId);
		//bool TryGetTeam(string nflId, WeekInfo week, out int teamId);
	}
	
	public class PlayerWeekTeamHistory : IPlayerWeekTeamHistory
	{
		private PlayerWeekTeamMap _map { get; }

		public PlayerWeekTeamHistory(PlayerWeekTeamMap map)
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

		//public bool TryGetTeam(string nflId, int season, int week, out int teamId)
		//{
		//	var weekInfo = new WeekInfo(season, week);
		//	return TryGetTeam(nflId, weekInfo, out teamId);
		//}

		//public bool TryGetTeam(string nflId, WeekInfo week, out int teamId)
		//{
		//	Dictionary<string, Dictionary<WeekInfo, int>> map = _map.Get();

		//	if (!map.TryGetValue(nflId, out Dictionary<WeekInfo, int> weekMap)
		//		|| !weekMap.TryGetValue(week, out int id))
		//	{
		//		teamId = -1;
		//		return false;
		//	}

		//	teamId = id;
		//	return true;
		//}
	}
}
