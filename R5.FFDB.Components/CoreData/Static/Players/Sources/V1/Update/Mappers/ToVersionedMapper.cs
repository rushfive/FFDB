using R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Update.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Update.Mappers
{
	public interface IToVersionedMapper : IAsyncMapper<string, PlayerUpdateVersioned, string> { }

	public class ToVersionedMapper : IToVersionedMapper
	{
		public Task<PlayerUpdateVersioned> MapAsync(string httpResponse, string nflId)
		{
			throw new NotImplementedException();
		}
	}
}
