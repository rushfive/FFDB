using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models.Entities
{
	[TableName("ffdb.week_stats_dst")]
	public class WeekStatsDstSql : WeekStatsSqlBase
	{
		[WeekStatColumn("sacks", WeekStatType.DST_Sacks)]
		public double? Sacks { get; set; }

		[WeekStatColumn("interceptions", WeekStatType.DST_Interceptions)]
		public double? Interceptions { get; set; }

		[WeekStatColumn("fumbles_recovered", WeekStatType.DST_FumblesRecovered)]
		public double? FumblesRecovered { get; set; }

		[WeekStatColumn("fumbles_forced", WeekStatType.DST_FumblesForced)]
		public double? FumblesForced { get; set; }

		[WeekStatColumn("safeties", WeekStatType.DST_Safeties)]
		public double? Safeties { get; set; }

		[WeekStatColumn("touchdowns", WeekStatType.DST_Touchdowns)]
		public double? Touchdowns { get; set; }

		[WeekStatColumn("blocked_kicks", WeekStatType.DST_BlockedKicks)]
		public double? BlockedKicks { get; set; }

		[WeekStatColumn("return_yards", WeekStatType.DST_ReturnYards)]
		public double? ReturnYards { get; set; }

		[WeekStatColumn("return_touchdowns", WeekStatType.DST_ReturnTouchdowns)]
		public double? ReturnTouchdowns { get; set; }

		[WeekStatColumn("points_allowed", WeekStatType.DST_PointsAllowed)]
		public double? PointsAllowed { get; set; }

		[WeekStatColumn("yards_allowed", WeekStatType.DST_YardsAllowed)]
		public double? YardsAllowed { get; set; }

		public override string InsertCommand()
		{
			throw new NotImplementedException();
		}
	}
}
