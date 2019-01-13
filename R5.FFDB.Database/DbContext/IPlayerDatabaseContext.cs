using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R5.FFDB.Database.DbContext
{
	public interface IPlayerDatabaseContext
	{
		Task AddAsync(List<Player> players, List<Roster> rosters);
		Task UpdateAsync(List<Player> players, List<Roster> rosters);
		Task<List<Player>> GetAllAsync();
	}
}
