﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Models
{
	public struct WeekInfo : IComparable<WeekInfo>
	{
		//[JsonProperty("season")]
		public int Season { get; }

		//[JsonProperty("week")]
		public int Week { get; }

		//[JsonConstructor]
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

		public int CompareTo(WeekInfo other)
		{
			if (this.Season < other.Season)
			{
				return -1;
			}
			if (other.Season < this.Season)
			{
				return 1;
			}

			if (this.Week < other.Week)
			{
				return -1;
			}
			if (other.Week < this.Week)
			{
				return 1;
			}
			return 0;
		}

		public static bool operator ==(WeekInfo a, WeekInfo b)
		{
			return a.CompareTo(b) == 0;
		}

		public static bool operator !=(WeekInfo a, WeekInfo b)
		{
			return a.CompareTo(b) != 0;
		}

		public static bool operator <(WeekInfo a, WeekInfo b)
		{
			return a.CompareTo(b) == -1;
		}

		public static bool operator >(WeekInfo a, WeekInfo b)
		{
			return a.CompareTo(b) == 1;
		}

		public static bool operator <=(WeekInfo a, WeekInfo b)
		{
			return a.CompareTo(b) < 1;
		}

		public static bool operator >=(WeekInfo a, WeekInfo b)
		{
			return a.CompareTo(b) > -1;
		}
	}
}