using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models.Entities
{
	[TableName("ffdb.week_stats")]
	public class WeekStatsSql : WeekStatsPlayerSqlBase
	{
		[NotNull]
		[ForeignKey(typeof(PlayerSql), "id")]
		[Column("player_id", PostgresDataType.UUID)]
		public override Guid PlayerId { get; set; }

		// Can be null, as safety in case we can't resolve it from sources
		[ForeignKey(typeof(TeamSql), "id")]
		[Column("team_id", PostgresDataType.INT)]
		public override int? TeamId { get; set; }

		[NotNull]
		[Column("season", PostgresDataType.INT)]
		public override int Season { get; set; }

		[NotNull]
		[Column("week", PostgresDataType.INT)]
		public override int Week { get; set; }

		[WeekStatColumn("pass_attempts", WeekStatType.Pass_Attempts)]
		public double? PassAttempts { get; set; }

		[WeekStatColumn("pass_completions", WeekStatType.Pass_Completions)]
		public double? PassCompletions { get; set; }

		[WeekStatColumn("pass_yards", WeekStatType.Pass_Yards)]
		public double? PassYards { get; set; }

		[WeekStatColumn("pass_touchdowns", WeekStatType.Pass_Touchdowns)]
		public double? PassTouchdowns { get; set; }

		[WeekStatColumn("pass_interceptions", WeekStatType.Pass_Interceptions)]
		public double? PassInterceptions { get; set; }

		[WeekStatColumn("pass_sacked", WeekStatType.Pass_Sacked)]
		public double? PassSacked { get; set; }

		[WeekStatColumn("rush_attempts", WeekStatType.Rush_Attempts)]
		public double? RushAttempts { get; set; }

		[WeekStatColumn("rush_yards", WeekStatType.Rush_Yards)]
		public double? RushYards { get; set; }

		[WeekStatColumn("rush_touchdowns", WeekStatType.Rush_Touchdowns)]
		public double? RushTouchdowns { get; set; }

		[WeekStatColumn("receive_catches", WeekStatType.Receive_Catches)]
		public double? ReceiveCatches { get; set; }

		[WeekStatColumn("receive_yards", WeekStatType.Receive_Yards)]
		public double? ReceiveYards { get; set; }

		[WeekStatColumn("receive_touchdowns", WeekStatType.Receive_Touchdowns)]
		public double? ReceiveTouchdowns { get; set; }

		[WeekStatColumn("return_yards", WeekStatType.Return_Yards)]
		public double? ReturnYards { get; set; }

		[WeekStatColumn("return_touchdowns", WeekStatType.Return_Touchdowns)]
		public double? ReturnTouchdowns { get; set; }

		[WeekStatColumn("fumble_recover_touchdowns", WeekStatType.Fumble_Recover_Touchdowns)]
		public double? FumbleRecoverTouchdowns { get; set; }

		[WeekStatColumn("fumbles_lost", WeekStatType.Fumbles_Lost)]
		public double? FumblesLost { get; set; }

		[WeekStatColumn("fumbles_total", WeekStatType.Fumbles_Total)]
		public double? FumblesTotal { get; set; }

		[WeekStatColumn("two_point_conversions", WeekStatType.TwoPointConversions)]
		public double? TwoPointConversions { get; set; }
	}
}
