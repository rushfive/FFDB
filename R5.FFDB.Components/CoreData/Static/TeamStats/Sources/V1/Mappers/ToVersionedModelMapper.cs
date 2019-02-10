using R5.FFDB.Components.CoreData.Static.TeamStats.Sources.V1.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.TeamStats.Sources.V1.Mappers
{
	public interface IToVersionedModelMapper : IAsyncMapper<string, TeamStatsVersionedModel> { }

	public class ToVersionedModelMapper : IToVersionedModelMapper
	{
		public Task<TeamStatsVersionedModel> MapAsync(string input)
		{
			throw new NotImplementedException();
		}
	}
}
