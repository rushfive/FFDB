﻿using R5.FFDB.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R5.FFDB.Database.DbContext
{
	public interface IWeekStatsDatabaseContext
	{
		Task UpdateWeekAsync(WeekStats stats);
		Task UpdateWeeksAsync(List<WeekStats> stats);
		Task RemoveAllAsync();
		Task RemoveForWeekAsync(WeekInfo week);
	}
}
