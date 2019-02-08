using R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Update.Models;
using R5.FFDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Update.Mappers
{
	public class ToCoreDataMapper : IMapper<PlayerUpdateVersionedModel, Player>
	{
		public Player Map(PlayerUpdateVersionedModel versionedModel)
		{
			throw new NotImplementedException();
		}
	}
}
