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
	[Table(TableName.WeekStats.DST)]
	[CompositePrimaryKeys("team_id", "season", "week")]
	public class WeekStatsDstSql
	{
		[NotNull]
		[ForeignKey(typeof(TeamSql), "id")]
		[Column("team_id", PostgresDataType.INT)]
		public int TeamId { get; set; }

		[NotNull]
		[Column("season", PostgresDataType.INT)]
		public int Season { get; set; }

		[NotNull]
		[Column("week", PostgresDataType.INT)]
		public int Week { get; set; }

		[Column("sacks", PostgresDataType.FLOAT8)]
		public double? Sacks { get; set; }

		[Column("interceptions", PostgresDataType.FLOAT8)]
		public double? Interceptions { get; set; }

		[Column("fumbles_recovered", PostgresDataType.FLOAT8)]
		public double? FumblesRecovered { get; set; }

		[Column("fumbles_forced", PostgresDataType.FLOAT8)]
		public double? FumblesForced { get; set; }

		[Column("safeties", PostgresDataType.FLOAT8)]
		public double? Safeties { get; set; }

		[Column("touchdowns", PostgresDataType.FLOAT8)]
		public double? Touchdowns { get; set; }

		[Column("blocked_kicks", PostgresDataType.FLOAT8)]
		public double? BlockedKicks { get; set; }

		[Column("return_yards", PostgresDataType.FLOAT8)]
		public double? ReturnYards { get; set; }

		[Column("return_touchdowns", PostgresDataType.FLOAT8)]
		public double? ReturnTouchdowns { get; set; }

		[Column("points_allowed", PostgresDataType.FLOAT8)]
		public double? PointsAllowed { get; set; }

		[Column("yards_allowed", PostgresDataType.FLOAT8)]
		public double? YardsAllowed { get; set; }

		public void UpdateFromStats(List<KeyValuePair<WeekStatType, double>> stats)
		{
			foreach (KeyValuePair<WeekStatType, double> kv in stats)
			{
				switch (kv.Key)
				{
					case WeekStatType.DST_Sacks:
						this.Sacks = kv.Value;
						break;
					case WeekStatType.DST_Interceptions:
						this.Interceptions = kv.Value;
						break;
					case WeekStatType.DST_FumblesRecovered:
						this.FumblesRecovered = kv.Value;
						break;
					case WeekStatType.DST_FumblesForced:
						this.FumblesForced = kv.Value;
						break;
					case WeekStatType.DST_Safeties:
						this.Safeties = kv.Value;
						break;
					case WeekStatType.DST_Touchdowns:
						this.Touchdowns = kv.Value;
						break;
					case WeekStatType.DST_BlockedKicks:
						this.BlockedKicks = kv.Value;
						break;
					case WeekStatType.DST_ReturnYards:
						this.ReturnYards = kv.Value;
						break;
					case WeekStatType.DST_ReturnTouchdowns:
						this.ReturnTouchdowns = kv.Value;
						break;
					case WeekStatType.DST_PointsAllowed:
						this.PointsAllowed = kv.Value;
						break;
					case WeekStatType.DST_YardsAllowed:
						this.YardsAllowed = kv.Value;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(kv.Key), $"'{kv.Key}' is either an invalid or unhandled as a DST stat type.");
				}
			}
		}
	}
}
