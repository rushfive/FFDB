using R5.FFDB.Core.Models;
using R5.Lib.ValueProviders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.ValueProviders
{
	// todo: convert this to a cache?
	public class AvailableWeeksValue : AsyncValueProvider<List<WeekInfo>>
	{
		private LatestWeekValue _latestWeekValue { get; }

		public AvailableWeeksValue(LatestWeekValue latestWeekValue)
			: base("Available Weeks")
		{
			_latestWeekValue = latestWeekValue;
		}

		protected override async Task<List<WeekInfo>> ResolveValueAsync()
		{
			var result = new List<WeekInfo>();

			WeekInfo latest = await _latestWeekValue.GetAsync();

			// Earliest available is 2010-1
			for (int season = 2010; season < latest.Season; season++)
			{
				for (int week = 1; week <= 17; week++)
				{
					result.Add(new WeekInfo(season, week));
				}
			}

			for (int week = 1; week <= latest.Week; week++)
			{
				result.Add(new WeekInfo(latest.Season, week));
			}

			return result;
		}
	}
}
