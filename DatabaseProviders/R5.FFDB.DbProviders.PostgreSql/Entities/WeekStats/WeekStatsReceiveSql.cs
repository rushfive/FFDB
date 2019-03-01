using R5.FFDB.Core;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.Internals.PostgresMapper.Attributes;
using R5.Internals.PostgresMapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Entities.WeekStats
{
	[Table(TableName.WeekStats.Receive)]
	[CompositePrimaryKeys("player_id", "season", "week")]
	public class WeekStatsReceiveSql : WeekStatsPlayerSql
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

		[WeekStatColumn("receive_catches", WeekStatType.Receive_Catches)]
		public double? ReceiveCatches { get; set; }

		[WeekStatColumn("receive_yards", WeekStatType.Receive_Yards)]
		public double? ReceiveYards { get; set; }

		[WeekStatColumn("receive_touchdowns", WeekStatType.Receive_Touchdowns)]
		public double? ReceiveTouchdowns { get; set; }

		public override string PrimaryKeyMatchCondition()
		{
			return $"player_id = '{PlayerId}' AND season = {Season} AND week = {Week}";
		}
	}
}
