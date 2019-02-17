using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Components.CoreData.Dynamic.Rosters
{
	public class RosterCacheData
	{
		private List<Roster> _rosters { get; } = new List<Roster>();

		private Dictionary<string, (int?, Position?, RosterStatus?)> _playerDataMap { get; }
			= new Dictionary<string, (int?, Position?, RosterStatus?)>(StringComparer.OrdinalIgnoreCase);

		public void UpdateWith(Roster roster)
		{
			_rosters.Add(roster);

			foreach (var p in roster.Players)
			{
				_playerDataMap[p.NflId] = (p.Number, p.Position, p.Status);
			}
		}

		public List<Roster> GetRosters()
		{
			return _rosters.ToList();
		}

		public List<string> GetCurrentlyRosteredIds()
		{
			return _rosters
				.SelectMany(r => r.Players)
				.Select(p => p.NflId)
				.ToList();
		}

		public (int? number, Position? position, RosterStatus? status)? GetPlayerData(string nflId)
		{
			if (_playerDataMap.TryGetValue(nflId, out (int ?, Position ?, RosterStatus ?) data))
			{
				return data;
			}

			return null;
		}
	}
}
