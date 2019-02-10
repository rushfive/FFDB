using R5.FFDB.Components.CoreData.Static.PlayerStats.Sources.V1.Models;
using R5.FFDB.Core.Models;
using R5.FFDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.PlayerStats.Sources.V1.Mappers
{
	public interface IToCoreDataMapper : IAsyncMapper<PlayerWeekStatsVersioned, List<PlayerWeekStats>, WeekInfo> { }

	public class ToCoreDataMapper : IToCoreDataMapper
	{
		public Task<List<PlayerWeekStats>> MapAsync(PlayerWeekStatsVersioned versionedModel, WeekInfo week)
		{
			throw new NotImplementedException();
		}
	}
}
