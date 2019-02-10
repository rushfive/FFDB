using R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Update.Models;
using R5.FFDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Update.Mappers
{
	public interface IToCoreDataMapper : IAsyncMapper<PlayerUpdateVersionedModel, Player> { }

	public class ToCoreDataMapper : IToCoreDataMapper
	{
		public Task<Player> MapAsync(PlayerUpdateVersionedModel versionedModel)
		{
			throw new NotImplementedException();
		}
	}
}
