using R5.FFDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Components.CoreData.Static.TeamStats.Models
{
	public class TeamStatsCacheData
	{
		private List<TeamWeekStats> _stats { get; } = new List<TeamWeekStats>();
		private Dictionary<string, int> _playerTeamMap { get; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

		public void UpdateWith(TeamWeekStats stats)
		{
			_stats.Add(stats);

			foreach (var id in stats.PlayerNflIds)
			{
				_playerTeamMap[id] = stats.TeamId;
			}
		}

		public List<TeamWeekStats> GetStats()
		{
			return _stats.ToList();
		}

		public Dictionary<string, int> GetPlayerTeamMap()
		{
			return new Dictionary<string, int>(_playerTeamMap, StringComparer.OrdinalIgnoreCase);
		}
	}
}
