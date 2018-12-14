using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.TeamGameHistory.Sources
{
	public interface ITeamGameHistorySource : ISource
	{
		Task FetchAndSaveAsync();
	}
}
