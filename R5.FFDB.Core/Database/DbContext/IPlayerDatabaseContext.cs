using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R5.FFDB.Core.Database.DbContext
{
	public interface IPlayerDatabaseContext
	{
		// NEW for pipeline
		Task AddAsync(Player player);

		// OLD
		Task AddAsync(List<Player> players, List<Roster> rosters);
		Task UpdateAsync(List<Player> players, List<Roster> rosters);
		Task<List<Player>> GetAllAsync();
		Task<List<Player>> GetByTeamForWeekAsync(int teamId, WeekInfo week);
	}
}
