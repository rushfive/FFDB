using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Models
{
	public class WeekStats
	{
		public WeekInfo Week { get; set; }
		public List<PlayerStats> Players { get; set; }
	}

	public class PlayerStats
	{
		public string NflId { get; set; }
		public Dictionary<WeekStatType, double> Stats { get; set; }
		public int? TeamId { get; set; }
	}

	// key "pts" == null means DIDNT play
	public enum WeekStatType
	{
		// Passing
		Pass_Attempts = 2,
		Pass_Completions = 3,
		Pass_Yards = 5,
		Pass_Touchdowns = 6,
		Pass_Interceptions = 7,
		Pass_Sacked = 8,
		// Rushing
		Rush_Attempts = 13,
		Rush_Yards = 14,
		Rush_Touchdowns = 15,
		// Receiving
		Receive_Catches = 20, // targets?
		Receive_Yards = 21,
		Receive_Touchdowns = 22,
		// Misc
		Return_Yards = 27,
		Return_Touchdowns = 28,
		Fumble_Recover_Touchdowns = 29, // single player, eg mike evans week 12
		Fumbles_Lost = 30,
		Fumbles_Total = 31,
		TwoPointConversions = 32,
		// Kicking (totals implied by adding)
		Kick_PAT_Makes = 33,
		Kick_PAT_Misses = 34,
		Kick_ZeroTwenty_Makes = 35,
		Kick_TwentyThirty_Makes = 36,
		Kick_ThirtyForty_Makes = 37,
		Kick_FortyFifty_Makes = 38,
		Kick_FiftyPlus_Makes = 39,
		Kick_ZeroTwenty_Misses = 40,
		Kick_TwentyThirty_Misses = 41,
		Kick_ThirtyForty_Misses = 42,
		Kick_FortyFifty_Misses = 43,
		Kick_FiftyPlus_Misses = 44,
		// DST (double check this is DST and not IDP)
		DST_Sacks = 45,
		DST_Interceptions = 46,
		DST_FumblesRecovered = 47,
		DST_FumblesForced = 48,
		DST_Safeties = 49,
		DST_Touchdowns = 50,
		DST_BlockedKicks = 51,
		DST_ReturnYards = 52, // double check that this is DST
		DST_ReturnTouchdowns = 53,
		DST_PointsAllowed = 54,
		DST_YardsAllowed = 62,
		// IDP (double check this is IDP and not DST)
		IDP_Tackles = 70,
		IDP_AssistedTackles = 71,
		IDP_Sacks = 72,
		IDP_Interceptions = 73,
		IDP_ForcedFumbles = 74,
		IDP_FumblesRecovered = 75,
		IDP_InterceptionTouchdowns = 76,
		IDP_FumbleTouchdowns = 77,
		IDP_BlockedKickTouchdowns = 78, // blocked kicks
		IDP_BlockedKicks = 79, // punt, fg, pats
		IDP_Safeties = 80,
		IDP_PassesDefended = 81,
		IDP_InterceptionReturnYards = 82,
		IDP_FumbleReturnYards = 83,
		IDP_TacklesForLoss = 84, // check if this is count vs yards
		IDP_QuarterBackHits = 85,
		IDP_SackYards = 86
	}
}
