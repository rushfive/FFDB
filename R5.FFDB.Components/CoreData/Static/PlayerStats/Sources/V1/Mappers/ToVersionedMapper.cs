using R5.FFDB.Components.CoreData.Static.PlayerStats.Sources.V1.Models;
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
		public Task<PlayerWeekStatsVersioned> MapAsync(string httpResponse, WeekInfo week)
		{
			throw new NotImplementedException();
		}
	}
}
