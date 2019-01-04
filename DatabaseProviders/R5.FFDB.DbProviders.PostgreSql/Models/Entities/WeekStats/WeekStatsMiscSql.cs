using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models.Entities.WeekStats
{
	[TableName(Table.WeekStats.Misc)]
	[CompositePrimaryKeys("player_id", "season", "week")]
	public class WeekStatsMiscSql : WeekStatsPlayerSql
	{
		[NotNull]
		[ForeignKey(typeof(PlayerSql), "id")]
		[Column("player_id", PostgresDataType.UUID)]
		public override Guid PlayerId { get; set; }
		
		[ForeignKey(typeof(TeamSql), "id")]
		[Column("team_id", PostgresDataType.INT)]
		public override int? TeamId { get; set; }

		[NotNull]
		[Column("season", PostgresDataType.INT)]
		public override int Season { get; set; }

		[NotNull]
		[Column("week", PostgresDataType.INT)]
		public override int Week { get; set; }

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
