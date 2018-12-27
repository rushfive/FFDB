using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models.Entities
{
	[TableName("ffdb.week_stats")]
	public class WeekStatsSql : WeekStatsSqlBase
	{
		[WeekStatColumn("pass_attempts", WeekStatType.Pass_Attempts)]
		public double? PassAttempts { get; set; }

		[WeekStatColumn("pass_completions", WeekStatType.Pass_Completions)]
		public double? PassCompletions { get; set; }

		[WeekStatColumn("pass_yards", WeekStatType.Pass_Yards)]
		public double? PassYards { get; set; }

		[WeekStatColumn("pass_touchdowns", WeekStatType.Pass_Touchdowns)]
		public double? PassTouchdowns { get; set; }

		[WeekStatColumn("pass_interceptions", WeekStatType.Pass_Interceptions)]
		public double? PassInterceptions { get; set; }

		[WeekStatColumn("pass_sacked", WeekStatType.Pass_Sacked)]
		public double? PassSacked { get; set; }

		[WeekStatColumn("rush_attempts", WeekStatType.Rush_Attempts)]
		public double? RushAttempts { get; set; }

		[WeekStatColumn("rush_yards", WeekStatType.Rush_Yards)]
		public double? RushYards { get; set; }

		[WeekStatColumn("rush_touchdowns", WeekStatType.Rush_Touchdowns)]
		public double? RushTouchdowns { get; set; }

		[WeekStatColumn("receive_catches", WeekStatType.Receive_Catches)]
		public double? ReceiveCatches { get; set; }

		[WeekStatColumn("receive_yards", WeekStatType.Receive_Yards)]
		public double? ReceiveYards { get; set; }

		[WeekStatColumn("receive_touchdowns", WeekStatType.Receive_Touchdowns)]
		public double? ReceiveTouchdowns { get; set; }

		[WeekStatColumn("return_yards", WeekStatType.Return_Yards)]
		public double? ReturnYards { get; set; }

		[WeekStatColumn("return_touchdowns", WeekStatType.Return_Touchdowns)]
		public double? ReturnTouchdowns { get; set; }

		[WeekStatColumn("fumble_recover_touchdowns", WeekStatType.Fumble_Recover_Touchdowns)]
		public double? FumbleRecoverTouchdowns { get; set; }

		[WeekStatColumn("fumbles_lost", WeekStatType.Fumbles_Lost)]
		public double? FumblesLost { get; set; }

		[WeekStatColumn("fumbles_total", WeekStatType.Fumbles_Total)]
		public double? FumblesTotal { get; set; }

		[WeekStatColumn("two_point_conversions", WeekStatType.TwoPointConversions)]
		public double? TwoPointConversions { get; set; }

		//[WeekStatColumn("kick_pat_makes", WeekStatType.Kick_PAT_Makes)]
		//public double? KickPatMakes { get; set; }

		//[WeekStatColumn("kick_pat_misses", WeekStatType.Kick_PAT_Misses)]
		//public double? KickPatMisses { get; set; }

		//[WeekStatColumn("kick_zero_twenty_makes", WeekStatType.Kick_ZeroTwenty_Makes)]
		//public double? KickZeroTwentyMakes { get; set; }

		//[WeekStatColumn("kick_twenty_thirty_makes", WeekStatType.Kick_TwentyThirty_Makes)]
		//public double? KickTwentyThirtyMakes { get; set; }

		//[WeekStatColumn("kick_thirty_forty_makes", WeekStatType.Kick_ThirtyForty_Makes)]
		//public double? KickThirtyFortyMakes { get; set; }

		//[WeekStatColumn("kick_forty_fifty_makes", WeekStatType.Kick_FortyFifty_Makes)]
		//public double? KickFortyFiftyMakes { get; set; }

		//[WeekStatColumn("kick_fifty_plus_makes", WeekStatType.Kick_FiftyPlus_Makes)]
		//public double? KickFiftyPlusMakes { get; set; }

		//[WeekStatColumn("kick_zero_twenty_misses", WeekStatType.Kick_ZeroTwenty_Misses)]
		//public double? KickZeroTwentyMisses { get; set; }

		//[WeekStatColumn("kick_twenty_thirty_misses", WeekStatType.Kick_TwentyThirty_Misses)]
		//public double? KickTwentyThirtyMisses { get; set; }

		//[WeekStatColumn("kick_thirty_forty_misses", WeekStatType.Kick_ThirtyForty_Misses)]
		//public double? KickThirtyFortyMisses { get; set; }

		//[WeekStatColumn("kick_forty_fifty_misses", WeekStatType.Kick_FortyFifty_Misses)]
		//public double? KickFortyFiftyMisses { get; set; }

		//[WeekStatColumn("kick_fifty_plus_misses", WeekStatType.Kick_FiftyPlus_Misses)]
		//public double? KickFiftyPlusMisses { get; set; }

		//[WeekStatColumn("dst_sacks", WeekStatType.DST_Sacks)]
		//public double? DstSacks { get; set; }

		//[WeekStatColumn("dst_interceptions", WeekStatType.DST_Interceptions)]
		//public double? DstInterceptions { get; set; }

		//[WeekStatColumn("dst_fumbles_recovered", WeekStatType.DST_FumblesRecovered)]
		//public double? DstFumblesRecovered { get; set; }

		//[WeekStatColumn("dst_fumbles_forced", WeekStatType.DST_FumblesForced)]
		//public double? DstFumblesForced { get; set; }

		//[WeekStatColumn("dst_safeties", WeekStatType.DST_Safeties)]
		//public double? DstSafeties { get; set; }

		//[WeekStatColumn("dst_touchdowns", WeekStatType.DST_Touchdowns)]
		//public double? DstTouchdowns { get; set; }

		//[WeekStatColumn("dst_blocked_kicks", WeekStatType.DST_BlockedKicks)]
		//public double? DstBlockedKicks { get; set; }

		//[WeekStatColumn("dst_return_yards", WeekStatType.DST_ReturnYards)]
		//public double? DstReturnYards { get; set; }

		//[WeekStatColumn("dst_return_touchdowns", WeekStatType.DST_ReturnTouchdowns)]
		//public double? DstReturnTouchdowns { get; set; }

		//[WeekStatColumn("dst_points_allowed", WeekStatType.DST_PointsAllowed)]
		//public double? DstPointsAllowed { get; set; }

		//[WeekStatColumn("dst_yards_allowed", WeekStatType.DST_YardsAllowed)]
		//public double? DstYardsAllowed { get; set; }

		//[WeekStatColumn("idp_tackles", WeekStatType.IDP_Tackles)]
		//public double? IdpTackles { get; set; }

		//[WeekStatColumn("idp_assisted_tackles", WeekStatType.IDP_AssistedTackles)]
		//public double? IdpAssistedTackles { get; set; }

		//[WeekStatColumn("idp_sacks", WeekStatType.IDP_Sacks)]
		//public double? IdpSacks { get; set; }

		//[WeekStatColumn("idp_interceptions", WeekStatType.IDP_Interceptions)]
		//public double? IdpInterceptions { get; set; }

		//[WeekStatColumn("idp_forced_fumbles", WeekStatType.IDP_ForcedFumbles)]
		//public double? IdpForcedFumbles { get; set; }

		//[WeekStatColumn("idp_fumbles_recovered", WeekStatType.IDP_FumblesRecovered)]
		//public double? IdpFumblesRecovered { get; set; }

		//[WeekStatColumn("idp_interception_touchdowns", WeekStatType.IDP_InterceptionTouchdowns)]
		//public double? IdpInterceptionTouchdowns { get; set; }

		//[WeekStatColumn("idp_fumble_touchdowns", WeekStatType.IDP_FumbleTouchdowns)]
		//public double? IdpFumbleTouchdowns { get; set; }

		//[WeekStatColumn("idp_blocked_kick_touchdowns", WeekStatType.IDP_BlockedKickTouchdowns)]
		//public double? IdpBlockedKickTouchdowns { get; set; }

		//[WeekStatColumn("idp_blocked_kicks", WeekStatType.IDP_BlockedKicks)]
		//public double? IdpBlockedKicks { get; set; }

		//[WeekStatColumn("idp_safeties", WeekStatType.IDP_Safeties)]
		//public double? IdpSafeties { get; set; }

		//[WeekStatColumn("idp_passes_defended", WeekStatType.IDP_PassesDefended)]
		//public double? IdpPassesDefended { get; set; }

		//[WeekStatColumn("idp_interception_return_yards", WeekStatType.IDP_InterceptionReturnYards)]
		//public double? IdpInterceptionReturnYards { get; set; }

		//[WeekStatColumn("idp_fumble_return_yards", WeekStatType.IDP_FumbleReturnYards)]
		//public double? IdpFumbleReturnYards { get; set; }

		//[WeekStatColumn("idp_tackles_for_loss", WeekStatType.IDP_TacklesForLoss)]
		//public double? IdpTacklesForLoss { get; set; }

		//[WeekStatColumn("idp_quarterback_hits", WeekStatType.IDP_QuarterBackHits)]
		//public double? IdpQuarterbackHits { get; set; }

		//[WeekStatColumn("idp_sack_yards", WeekStatType.IDP_SackYards)]
		//public double? IdpSackYards { get; set; }

		public static WeekStatsSql FromCoreEntity(PlayerStats stats, 
			Guid playerId, WeekInfo week)
		{
			var result = new WeekStatsSql
			{
				PlayerId = playerId,
				TeamId = stats.TeamId,
				Season = week.Season,
				Week = week.Week
			};

			foreach (KeyValuePair<WeekStatType, double> kv in stats.Stats)
			{
				PropertyInfo property = EntityInfoMap.GetPropertyByStat(kv.Key);
				property.SetValue(result, kv.Value);
			}

			return result;
		}

		//public static WeekStatsSql FromCoreEntity(
		//	Guid playerId, int? teamId, int season, int week,
		//	Dictionary<WeekStatType, double> stats)
		//{
		//	var result = new WeekStatsSql
		//	{
		//		PlayerId = playerId,
		//		TeamId = teamId,
		//		Season = season,
		//		Week = week
		//	};

		//	foreach(KeyValuePair<WeekStatType, double> kv in stats)
		//	{
		//		PropertyInfo property = EntityInfoMap.GetPropertyByStat(kv.Key);
		//		property.SetValue(result, kv.Value);
		//	}

		//	return result;
		//}

		public override string InsertCommand()
		{
			return SqlCommandBuilder.Rows.Insert(this);
		}
	}
}
