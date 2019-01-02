using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.TeamGameHistory.Values;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Components.CoreData.TeamGameHistory
{
	public interface ITeamWeekStatsService
	{
		List<TeamWeekStats> Get();
	}

	public class TeamWeekStatsService : ITeamWeekStatsService
	{
		private ILogger<TeamWeekStatsService> _logger { get; }
		private TeamWeekStatsMapValue _teamWeekStatsMap { get; }

		public TeamWeekStatsService(
			ILogger<TeamWeekStatsService> logger,
			TeamWeekStatsMapValue teamWeekStatsMap)
		{
			_logger = logger;
			_teamWeekStatsMap = teamWeekStatsMap;
		}

		public List<TeamWeekStats> Get()
		{
			var result = new List<TeamWeekStats>();

			Dictionary<WeekInfo, Dictionary<int, TeamWeekStats>> map = _teamWeekStatsMap.Get();
			foreach(Dictionary<int, TeamWeekStats> innerMap in map.Values)
			{
				result.AddRange(innerMap.Values);
			}

			return result;
		}
	}
}
