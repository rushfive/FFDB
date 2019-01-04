using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models.Entities.WeekStats
{
	[TableName(Table.WeekStats.IDP)]
	[CompositePrimaryKeys("player_id", "season", "week")]
	public class WeekStatsIdpSql : WeekStatsPlayerSql
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

		[WeekStatColumn("tackles", WeekStatType.IDP_Tackles)]
		public double? Tackles { get; set; }

		[WeekStatColumn("assisted_tackles", WeekStatType.IDP_AssistedTackles)]
		public double? AssistedTackles { get; set; }

		[WeekStatColumn("sacks", WeekStatType.IDP_Sacks)]
		public double? Sacks { get; set; }

		[WeekStatColumn("interceptions", WeekStatType.IDP_Interceptions)]
		public double? Interceptions { get; set; }

		[WeekStatColumn("forced_fumbles", WeekStatType.IDP_ForcedFumbles)]
		public double? ForcedFumbles { get; set; }

		[WeekStatColumn("fumbles_recovered", WeekStatType.IDP_FumblesRecovered)]
		public double? FumblesRecovered { get; set; }

		[WeekStatColumn("interception_touchdowns", WeekStatType.IDP_InterceptionTouchdowns)]
		public double? InterceptionTouchdowns { get; set; }

		[WeekStatColumn("fumble_touchdowns", WeekStatType.IDP_FumbleTouchdowns)]
		public double? FumbleTouchdowns { get; set; }

		[WeekStatColumn("blocked_kick_touchdowns", WeekStatType.IDP_BlockedKickTouchdowns)]
		public double? BlockedKickTouchdowns { get; set; }

		[WeekStatColumn("blocked_kicks", WeekStatType.IDP_BlockedKicks)]
		public double? BlockedKicks { get; set; }

		[WeekStatColumn("safeties", WeekStatType.IDP_Safeties)]
		public double? Safeties { get; set; }

		[WeekStatColumn("passes_defended", WeekStatType.IDP_PassesDefended)]
		public double? PassesDefended { get; set; }

		[WeekStatColumn("interception_return_yards", WeekStatType.IDP_InterceptionReturnYards)]
		public double? InterceptionReturnYards { get; set; }

		[WeekStatColumn("fumble_return_yards", WeekStatType.IDP_FumbleReturnYards)]
		public double? FumbleReturnYards { get; set; }

		[WeekStatColumn("tackles_for_loss", WeekStatType.IDP_TacklesForLoss)]
		public double? TacklesForLoss { get; set; }

		[WeekStatColumn("quarterback_hits", WeekStatType.IDP_QuarterBackHits)]
		public double? QuarterbackHits { get; set; }

		[WeekStatColumn("sack_yards", WeekStatType.IDP_SackYards)]
		public double? SackYards { get; set; }
	}
}
