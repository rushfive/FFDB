using R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Update.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Update.Mappers
{
	public class ToVersionedModelMapper : IMapper<string, PlayerUpdateVersionedModel>
	{
		public PlayerUpdateVersionedModel Map(string httpResponse)
		{
			throw new NotImplementedException();
		}
	}
}
