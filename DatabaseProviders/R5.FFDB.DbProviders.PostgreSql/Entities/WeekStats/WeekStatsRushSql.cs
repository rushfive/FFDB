﻿using R5.FFDB.Core;
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
	[Table(TableName.WeekStats.Rush)]
	[CompositePrimaryKeys("player_id", "season", "week")]
	public class WeekStatsRushSql
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

		[Column("attempts", PostgresDataType.FLOAT8)]
		public double? Attempts { get; set; }

		[Column("yards", PostgresDataType.FLOAT8)]
		public double? Yards { get; set; }

		[Column("touchdowns", PostgresDataType.FLOAT8)]
		public double? Touchdowns { get; set; }

		public void UpdateFromStats(List<KeyValuePair<WeekStatType, double>> stats)
		{
			foreach (KeyValuePair<WeekStatType, double> kv in stats)
			{
				switch (kv.Key)
				{
					case WeekStatType.Rush_Attempts:
						this.Attempts = kv.Value;
						break;
					case WeekStatType.Rush_Yards:
						this.Yards = kv.Value;
						break;
					case WeekStatType.Rush_Touchdowns:
						this.Touchdowns = kv.Value;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(kv.Key), $"'{kv.Key}' is either an invalid or unhandled as a rushing stat type.");
				}
			}
		}
	}
}
