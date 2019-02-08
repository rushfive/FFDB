using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1
{
	public interface IRosterCache
	{
		Task<(int? number, Position? position, RosterStatus? status)> GetPlayerDataAsync(string nflId);
	}

	public class RosterCache : IRosterCache
	{
		public Task<(int? number, Position? position, RosterStatus? status)> GetPlayerDataAsync(string nflId)
		{
			throw new NotImplementedException();
		}
	}
}
