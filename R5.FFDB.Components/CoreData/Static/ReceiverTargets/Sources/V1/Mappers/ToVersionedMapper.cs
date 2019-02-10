using R5.FFDB.Components.CoreData.Static.ReceiverTargets.Sources.V1.Models;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.ReceiverTargets.Sources.V1.Mappers
{
	public interface IToVersionedMapper : IAsyncMapper<string, ReceiverTargetsVersioned, string> { }

	public class ToVersionedMapper : IToVersionedMapper
	{
		public Task<ReceiverTargetsVersioned> MapAsync(string httpResponse, string gameId)
		{
			throw new NotImplementedException();
		}
	}
}
