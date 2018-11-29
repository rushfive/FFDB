using System.Collections.Generic;
using System.Threading.Tasks;

namespace R5.FFDB.Components.PlayerProfile
{
	public interface IPlayerProfileSource : ISource
	{
		Core.Models.PlayerProfile GetPlayerProfile(string nflId);
		List<Core.Models.PlayerProfile> GetPlayerProfile(List<string> nflIds);
		Task FetchAndSavePlayerDataFilesAsync(List<string> playerNflIds);
	}
}
