using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.Players
{
	public interface IPlayerIdMappings
	{
		Task<Dictionary<string, string>> GetGsisToNflMapAsync();
	}

	public class PlayerIdMappings : IPlayerIdMappings
	{
		public Task<Dictionary<string, string>> GetGsisToNflMapAsync()
		{
			throw new NotImplementedException();
		}
	}
}
