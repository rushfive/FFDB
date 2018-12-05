using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.PlayerTeamHistory
{
	public interface IPlayerTeamHistorySource : ISource
	{
		Task FetchAndSaveAsync(List<Core.Models.PlayerProfile> players);
	}
}
