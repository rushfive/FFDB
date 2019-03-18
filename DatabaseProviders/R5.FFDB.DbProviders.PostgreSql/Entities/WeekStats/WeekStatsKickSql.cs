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
	[Table(TableName.WeekStats.Kick)]
	[CompositePrimaryKeys("player_id", "season", "week")]
	public class WeekStatsKickSql
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

		[Column("pat_makes", PostgresDataType.FLOAT8)]
		public double? PatMakes { get; set; }

		[Column("pat_misses", PostgresDataType.FLOAT8)]
		public double? PatMisses { get; set; }

		[Column("zero_twenty_makes", PostgresDataType.FLOAT8)]
		public double? ZeroTwentyMakes { get; set; }

		[Column("twenty_thirty_makes", PostgresDataType.FLOAT8)]
		public double? TwentyThirtyMakes { get; set; }

		[Column("thirty_forty_makes", PostgresDataType.FLOAT8)]
		public double? ThirtyFortyMakes { get; set; }

		[Column("forty_fifty_makes", PostgresDataType.FLOAT8)]
		public double? FortyFiftyMakes { get; set; }

		[Column("fifty_plus_makes", PostgresDataType.FLOAT8)]
		public double? FiftyPlusMakes { get; set; }

		[Column("zero_twenty_misses", PostgresDataType.FLOAT8)]
		public double? ZeroTwentyMisses { get; set; }

		[Column("twenty_thirty_misses", PostgresDataType.FLOAT8)]
		public double? TwentyThirtyMisses { get; set; }

		[Column("thirty_forty_misses", PostgresDataType.FLOAT8)]
		public double? ThirtyFortyMisses { get; set; }

		[Column("forty_fifty_misses", PostgresDataType.FLOAT8)]
		public double? FortyFiftyMisses { get; set; }

		[Column("fifty_plus_misses", PostgresDataType.FLOAT8)]
		public double? FiftyPlusMisses { get; set; }

		public void UpdateFromStats(List<KeyValuePair<WeekStatType, double>> stats)
		{
			foreach (KeyValuePair<WeekStatType, double> kv in stats)
			{
				switch (kv.Key)
				{
					case WeekStatType.Kick_PAT_Makes:
						this.PatMakes = kv.Value;
						break;
					case WeekStatType.Kick_PAT_Misses:
						this.PatMisses = kv.Value;
						break;
					case WeekStatType.Kick_ZeroTwenty_Makes:
						this.ZeroTwentyMakes = kv.Value;
						break;
					case WeekStatType.Kick_TwentyThirty_Makes:
						this.TwentyThirtyMakes = kv.Value;
						break;
					case WeekStatType.Kick_ThirtyForty_Makes:
						this.ThirtyFortyMakes = kv.Value;
						break;
					case WeekStatType.Kick_FortyFifty_Makes:
						this.FortyFiftyMakes = kv.Value;
						break;
					case WeekStatType.Kick_FiftyPlus_Makes:
						this.FiftyPlusMakes = kv.Value;
						break;
					case WeekStatType.Kick_ZeroTwenty_Misses:
						this.ZeroTwentyMisses = kv.Value;
						break;
					case WeekStatType.Kick_TwentyThirty_Misses:
						this.TwentyThirtyMisses = kv.Value;
						break;
					case WeekStatType.Kick_ThirtyForty_Misses:
						this.ThirtyFortyMisses = kv.Value;
						break;
					case WeekStatType.Kick_FortyFifty_Misses:
						this.FortyFiftyMisses = kv.Value;
						break;
					case WeekStatType.Kick_FiftyPlus_Misses:
						this.FiftyPlusMisses = kv.Value;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(kv.Key), $"'{kv.Key}' is either an invalid or unhandled as a kicking stat type.");
				}
			}
		}
	}
}
