using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Abstractions
{
	public struct WeekInfo
	{
		public int Season { get; }
		public int Week { get; }

		public WeekInfo(int season, int week)
		{
			Season = season;
			Week = week;
		}

		public override bool Equals(object other)
		{
			if (!(other is WeekInfo))
			{
				return false;
			}

			var otherWeek = (WeekInfo)other;

			return Season == otherWeek.Season && Week == otherWeek.Week;
		}

		public override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 23 + Season.GetHashCode();
			hash = hash * 23 + Week.GetHashCode();
			return hash;
		}

		public override string ToString()
		{
			return $"{Season}-{Week}";
		}
	}
}
