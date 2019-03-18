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
	[Table(TableName.WeekStats.Misc)]
	[CompositePrimaryKeys("player_id", "season", "week")]
	public class WeekStatsMiscSql
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

		[Column("fumble_recover_touchdowns", PostgresDataType.FLOAT8)]
		public double? FumbleRecoverTouchdowns { get; set; }

		[Column("fumbles_lost", PostgresDataType.FLOAT8)]
		public double? FumblesLost { get; set; }

		[Column("fumbles_total", PostgresDataType.FLOAT8)]
		public double? FumblesTotal { get; set; }

		[Column("two_point_conversions", PostgresDataType.FLOAT8)]
		public double? TwoPointConversions { get; set; }

		public void UpdateFromStats(List<KeyValuePair<WeekStatType, double>> stats)
		{
			foreach (KeyValuePair<WeekStatType, double> kv in stats)
			{
				switch (kv.Key)
				{
					case WeekStatType.Fumble_Recover_Touchdowns:
						this.FumbleRecoverTouchdowns = kv.Value;
						break;
					case WeekStatType.Fumbles_Lost:
						this.FumblesLost = kv.Value;
						break;
					case WeekStatType.Fumbles_Total:
						this.FumblesTotal = kv.Value;
						break;
					case WeekStatType.TwoPointConversions:
						this.TwoPointConversions = kv.Value;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(kv.Key), $"'{kv.Key}' is either an invalid or unhandled as a misc stat type.");
				}
			}
		}
	}
}
