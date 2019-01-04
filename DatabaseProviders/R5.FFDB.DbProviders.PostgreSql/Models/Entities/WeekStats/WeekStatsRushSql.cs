using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models.Entities.WeekStats
{
	[TableName(Table.WeekStats.Rush)]
	[CompositePrimaryKeys("player_id", "season", "week")]
	public class WeekStatsRushSql : WeekStatsPlayerSql
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

		[WeekStatColumn("rush_attempts", WeekStatType.Rush_Attempts)]
		public double? RushAttempts { get; set; }

		[WeekStatColumn("rush_yards", WeekStatType.Rush_Yards)]
		public double? RushYards { get; set; }

		[WeekStatColumn("rush_touchdowns", WeekStatType.Rush_Touchdowns)]
		public double? RushTouchdowns { get; set; }
	}
}
