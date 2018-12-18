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
		int GetTeam(string nflId, int season, int week);
		int GetTeam(string nflId, WeekInfo week);
	}
	
	public class PlayerWeekTeamHistory : IPlayerWeekTeamHistory
	{
		private PlayerWeekTeamMap _map { get; }

		public PlayerWeekTeamHistory(PlayerWeekTeamMap map)
		{
			_map = map;
		}

		public int GetTeam(string nflId, int season, int week)
		{
			var weekInfo = new WeekInfo(season, week);
			return GetTeam(nflId, weekInfo);
		}

		public int GetTeam(string nflId, WeekInfo week)
		{
			Dictionary<string, Dictionary<WeekInfo, int>> map = _map.Get();

			if (!map.TryGetValue(nflId, out Dictionary<WeekInfo, int> weekMap))
			{
				throw new InvalidOperationException($"Failed to find team history for player '{nflId}'.");
			}
			if (!weekMap.TryGetValue(week, out int teamId))
			{
				throw new InvalidOperationException($"Failed to find team history for player '{nflId}' ({week.Season}-{week.Week})");
			}

			return teamId;
		}
	}
}
