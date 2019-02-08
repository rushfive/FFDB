using R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Add.Models;
using R5.FFDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Add.Mappers
{
	public class ToCoreDataMapper : IMapper<PlayerAddVersionedModel, PlayerAdd>
	{
		public PlayerAdd Map(PlayerAddVersionedModel versionedModel)
		{
			throw new NotImplementedException();
		}
	}
}
