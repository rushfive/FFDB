using R5.FFDB.Components.CoreData.Static.ReceiverTargets.Sources.V1.Models;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.ReceiverTargets.Sources.V1.Mappers
{
	public interface IToCoreMapper : IAsyncMapper<ReceiverTargetsVersioned, Dictionary<string, int>, string> { }

	public class ToCoreMapper : IToCoreMapper
	{
		public Task<Dictionary<string, int>> MapAsync(ReceiverTargetsVersioned versioned, string gameId)
		{
			throw new NotImplementedException();
		}
	}
}
