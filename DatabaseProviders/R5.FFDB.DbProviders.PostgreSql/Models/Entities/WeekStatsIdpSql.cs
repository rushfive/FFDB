using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models.Entities
{
	[TableName("ffdb.week_stats_idp")]
	public class WeekStatsIdpSql : WeekStatsSqlBase
	{
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

		public override string InsertCommand()
		{
			throw new NotImplementedException();
		}
	}
}
