using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models.Entities
{
	[TableName("ffdb.week_stats_kicker")]
	public class WeekStatsKickerSql : WeekStatsSqlBase
	{
		[WeekStatColumn("pat_makes", WeekStatType.Kick_PAT_Makes)]
		public double? PatMakes { get; set; }

		[WeekStatColumn("pat_misses", WeekStatType.Kick_PAT_Misses)]
		public double? PatMisses { get; set; }

		[WeekStatColumn("zero_twenty_makes", WeekStatType.Kick_ZeroTwenty_Makes)]
		public double? ZeroTwentyMakes { get; set; }

		[WeekStatColumn("twenty_thirty_makes", WeekStatType.Kick_TwentyThirty_Makes)]
		public double? TwentyThirtyMakes { get; set; }

		[WeekStatColumn("thirty_forty_makes", WeekStatType.Kick_ThirtyForty_Makes)]
		public double? ThirtyFortyMakes { get; set; }

		[WeekStatColumn("forty_fifty_makes", WeekStatType.Kick_FortyFifty_Makes)]
		public double? FortyFiftyMakes { get; set; }

		[WeekStatColumn("fifty_plus_makes", WeekStatType.Kick_FiftyPlus_Makes)]
		public double? FiftyPlusMakes { get; set; }

		[WeekStatColumn("zero_twenty_misses", WeekStatType.Kick_ZeroTwenty_Misses)]
		public double? ZeroTwentyMisses { get; set; }

		[WeekStatColumn("twenty_thirty_misses", WeekStatType.Kick_TwentyThirty_Misses)]
		public double? TwentyThirtyMisses { get; set; }

		[WeekStatColumn("thirty_forty_misses", WeekStatType.Kick_ThirtyForty_Misses)]
		public double? ThirtyFortyMisses { get; set; }

		[WeekStatColumn("forty_fifty_misses", WeekStatType.Kick_FortyFifty_Misses)]
		public double? FortyFiftyMisses { get; set; }

		[WeekStatColumn("fifty_plus_misses", WeekStatType.Kick_FiftyPlus_Misses)]
		public double? FiftyPlusMisses { get; set; }

		public override string InsertCommand()
		{
			throw new NotImplementedException();
		}
	}
}
