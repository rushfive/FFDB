using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R5.FFDB.Core.Database.DbContext
{
	public interface ITeamDatabaseContext
	{
		Task AddTeamsAsync();
		Task UpdateRostersAsync(List<Roster> rosters);
		Task AddGameStatsAsync(List<TeamWeekStats> stats);
		Task RemoveAllGameStatsAsync();
		Task RemoveGameStatsForWeekAsync(WeekInfo week);
		Task AddGameMatchupsAsync(List<WeekGameMatchup> gameMatchups);
	}
}
