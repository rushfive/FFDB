using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.WeekStats
{
	public interface IWeekStatsSource : ISource
	{
		Core.Models.WeekStats GetStats(WeekInfo week);
		Task SaveWeekStatFilesAsync();
	}
}
