using R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Add.Models;
using R5.FFDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Add.Mappers
{
	public interface IToCoreDataMapper : IAsyncMapper<PlayerAddVersionedModel, PlayerAdd, string> { }

	public class ToCoreDataMapper : IToCoreDataMapper
	{
		public Task<PlayerAdd> MapAsync(PlayerAddVersionedModel versionedModel, string nflId)
		{
			throw new NotImplementedException();
		}
	}
}
