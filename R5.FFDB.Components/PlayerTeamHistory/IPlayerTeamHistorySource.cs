using R5.FFDB.Components.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.PlayerTeamHistory
{
	public interface IPlayerTeamHistorySource : ISource
	{
		PlayerTeamHistoryStore GetHistoryStore();
		Task FetchAndSaveAsync(List<Core.Models.PlayerProfile> players);
	}
}
