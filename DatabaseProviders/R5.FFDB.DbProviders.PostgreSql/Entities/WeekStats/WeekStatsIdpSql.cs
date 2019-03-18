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
	[Table(TableName.WeekStats.IDP)]
	[CompositePrimaryKeys("player_id", "season", "week")]
	public class WeekStatsIdpSql
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

		[Column("tackles", PostgresDataType.FLOAT8)]
		public double? Tackles { get; set; }

		[Column("assisted_tackles", PostgresDataType.FLOAT8)]
		public double? AssistedTackles { get; set; }

		[Column("sacks", PostgresDataType.FLOAT8)]
		public double? Sacks { get; set; }

		[Column("interceptions", PostgresDataType.FLOAT8)]
		public double? Interceptions { get; set; }

		[Column("forced_fumbles", PostgresDataType.FLOAT8)]
		public double? ForcedFumbles { get; set; }

		[Column("fumbles_recovered", PostgresDataType.FLOAT8)]
		public double? FumblesRecovered { get; set; }

		[Column("interception_touchdowns", PostgresDataType.FLOAT8)]
		public double? InterceptionTouchdowns { get; set; }

		[Column("fumble_touchdowns", PostgresDataType.FLOAT8)]
		public double? FumbleTouchdowns { get; set; }

		[Column("blocked_kick_touchdowns", PostgresDataType.FLOAT8)]
		public double? BlockedKickTouchdowns { get; set; }

		[Column("blocked_kicks", PostgresDataType.FLOAT8)]
		public double? BlockedKicks { get; set; }

		[Column("safeties", PostgresDataType.FLOAT8)]
		public double? Safeties { get; set; }

		[Column("passes_defended", PostgresDataType.FLOAT8)]
		public double? PassesDefended { get; set; }

		[Column("interception_return_yards", PostgresDataType.FLOAT8)]
		public double? InterceptionReturnYards { get; set; }

		[Column("fumble_return_yards", PostgresDataType.FLOAT8)]
		public double? FumbleReturnYards { get; set; }

		[Column("tackles_for_loss", PostgresDataType.FLOAT8)]
		public double? TacklesForLoss { get; set; }

		[Column("quarterback_hits", PostgresDataType.FLOAT8)]
		public double? QuarterbackHits { get; set; }

		[Column("sack_yards", PostgresDataType.FLOAT8)]
		public double? SackYards { get; set; }

		public void UpdateFromStats(List<KeyValuePair<WeekStatType, double>> stats)
		{
			foreach (KeyValuePair<WeekStatType, double> kv in stats)
			{
				switch (kv.Key)
				{
					case WeekStatType.IDP_Tackles:
						this.Tackles = kv.Value;
						break;
					case WeekStatType.IDP_AssistedTackles:
						this.AssistedTackles = kv.Value;
						break;
					case WeekStatType.IDP_Sacks:
						this.Sacks = kv.Value;
						break;
					case WeekStatType.IDP_Interceptions:
						this.Interceptions = kv.Value;
						break;
					case WeekStatType.IDP_ForcedFumbles:
						this.ForcedFumbles = kv.Value;
						break;
					case WeekStatType.IDP_FumblesRecovered:
						this.FumblesRecovered = kv.Value;
						break;
					case WeekStatType.IDP_InterceptionTouchdowns:
						this.InterceptionTouchdowns = kv.Value;
						break;
					case WeekStatType.IDP_FumbleTouchdowns:
						this.FumbleTouchdowns = kv.Value;
						break;
					case WeekStatType.IDP_BlockedKickTouchdowns:
						this.BlockedKickTouchdowns = kv.Value;
						break;
					case WeekStatType.IDP_BlockedKicks:
						this.BlockedKicks = kv.Value;
						break;
					case WeekStatType.IDP_Safeties:
						this.Safeties = kv.Value;
						break;
					case WeekStatType.IDP_PassesDefended:
						this.PassesDefended = kv.Value;
						break;
					case WeekStatType.IDP_InterceptionReturnYards:
						this.InterceptionReturnYards = kv.Value;
						break;
					case WeekStatType.IDP_FumbleReturnYards:
						this.FumbleReturnYards = kv.Value;
						break;
					case WeekStatType.IDP_TacklesForLoss:
						this.TacklesForLoss = kv.Value;
						break;
					case WeekStatType.IDP_QuarterBackHits:
						this.QuarterbackHits = kv.Value;
						break;
					case WeekStatType.IDP_SackYards:
						this.SackYards = kv.Value;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(kv.Key), $"'{kv.Key}' is either an invalid or unhandled as an IDP stat type.");
				}
			}
		}
	}
}
