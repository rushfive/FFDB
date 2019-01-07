using R5.FFDB.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R5.FFDB.Database.DbContext
{
	public interface ILogDatabaseContext
	{
		Task AddUpdateForWeekAsync(WeekInfo week);
		Task<List<WeekInfo>> GetUpdatedWeeksAsync();
	}
}
