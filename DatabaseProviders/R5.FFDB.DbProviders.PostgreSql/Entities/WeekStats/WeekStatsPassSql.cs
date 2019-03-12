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
	[Table(TableName.WeekStats.Pass)]
	[CompositePrimaryKeys("player_id", "season", "week")]
	public class WeekStatsPassSql
	{
		[NotNull]
		[ForeignKey(typeof(PlayerSql), "id")]
		[Column("player_id", PostgresDataType.UUID)]
		public Guid PlayerId { get; set; }
		
		[ForeignKey(typeof(TeamSql), "id")]
		[Column("team_id", PostgresDataType.INT)]
		public int? TeamId { get; set; }

		[NotNull]
		[Column("season", PostgresDataType.INT)]
		public int Season { get; set; }

		[NotNull]
		[Column("week", PostgresDataType.INT)]
		public int Week { get; set; }

		[Column("pass_attempts", PostgresDataType.FLOAT8)]
		public double? PassAttempts { get; set; }

		[Column("pass_completions", PostgresDataType.FLOAT8)]
		public double? PassCompletions { get; set; }

		[Column("pass_yards", PostgresDataType.FLOAT8)]
		public double? PassYards { get; set; }

		[Column("pass_touchdowns", PostgresDataType.FLOAT8)]
		public double? PassTouchdowns { get; set; }

		[Column("pass_interceptions", PostgresDataType.FLOAT8)]
		public double? PassInterceptions { get; set; }

		[Column("pass_sacked", PostgresDataType.FLOAT8)]
		public double? PassSacked { get; set; }
	}
}
