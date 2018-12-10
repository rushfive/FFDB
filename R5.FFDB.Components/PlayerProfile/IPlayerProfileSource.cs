using System.Collections.Generic;
using System.Threading.Tasks;

namespace R5.FFDB.Components.PlayerProfile
{
	public interface IPlayerProfileSource : ISource
	{
		List<Core.Models.PlayerProfile> GetAll();
		Core.Models.PlayerProfile GetPlayerProfile(string nflId);
		List<Core.Models.PlayerProfile> GetPlayerProfile(List<string> nflIds);
		Task FetchAndSaveAsync(List<string> playerNflIds);
	}
}
