using System.Collections.Generic;
using System.Threading.Tasks;

namespace R5.FFDB.Components.PlayerData
{
	public interface IPlayerDataSource : ISource
	{
		Core.Models.PlayerData GetPlayerData(string nflId);
		List<Core.Models.PlayerData> GetPlayerData(List<string> nflIds);
		Task SavePlayerDataFilesAsync(List<(string nflId, string firstName, string lastName)> players);
	}
}
