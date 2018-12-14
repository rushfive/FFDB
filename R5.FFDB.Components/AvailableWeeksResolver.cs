using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components
{
	public interface IAvailableWeeksResolver
	{
		Task<List<WeekInfo>> GetAsync(HashSet<WeekInfo> excludeWeeks = null);
	}
	public class AvailableWeeksResolver : IAvailableWeeksResolver
	{
		private LatestWeekValue _latestWeek { get; }

		public AvailableWeeksResolver(LatestWeekValue latestWeek)
		{
			_latestWeek = latestWeek;
		}

		public async Task<List<WeekInfo>> GetAsync(HashSet<WeekInfo> excludeWeeks = null)
		{
			var result = new List<WeekInfo>();

			WeekInfo latest = await _latestWeek.GetAsync();
			
			// Earliest available is 2010-1
			for (int season = 2010; season < latest.Season; season++)
			{
				for (int week = 1; week <= 17; week++)
				{
					var weekInfo = new WeekInfo(season, week);
					if (excludeWeeks != null && !excludeWeeks.Contains(weekInfo))
					{
						result.Add(weekInfo);
					}
				}
			}

			for (int week = 1; week <= latest.Week; week++)
			{
				var weekInfo = new WeekInfo(latest.Season, week);
				if (excludeWeeks != null && !excludeWeeks.Contains(weekInfo))
				{
					result.Add(weekInfo);
				}
			}

			return result;
		}
	}

	public static class ListExtensions
	{
		public static List<T> AddIf<T>(this List<T> list, 
			T item, Func<bool> predicate)
		{
			if (predicate())
			{
				list.Add(item);
			}

			return list;
		}
	}
}
