using R5.FFDB.Components.CoreData.Static.PlayerStats.Sources.V1.Models;
using R5.FFDB.Core.Models;
using R5.FFDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.PlayerStats.Sources.V1.Mappers
{
	public interface IToCoreMapper : IAsyncMapper<PlayerWeekStatsVersioned, List<PlayerWeekStats>, WeekInfo> { }

	public class ToCoreMapper : IToCoreMapper
	{
		public Task<List<PlayerWeekStats>> MapAsync(PlayerWeekStatsVersioned versionedModel, WeekInfo week)
		{
			var result = new List<PlayerWeekStats>();

			foreach(var p in versionedModel.Players)
			{
				result.Add(new PlayerWeekStats
				{
					Week = week,
					NflId = p.NflId,
					Stats = p.Stats,
					TeamId = p.TeamId
				});
			}
			
			return Task.FromResult(result);
		}
	}
}
