using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core
{
	/// <summary>
	/// Contains sets of all the recorded stat types, grouped by categories.
	/// </summary>
	public static class WeekStatCategory
	{
		/// <summary>
		/// Set of all the different passing stat types.
		/// </summary>
		public static HashSet<WeekStatType> Pass = new HashSet<WeekStatType>
		{
			WeekStatType.Pass_Attempts,
			WeekStatType.Pass_Completions,
			WeekStatType.Pass_Yards,
			WeekStatType.Pass_Touchdowns,
			WeekStatType.Pass_Interceptions,
			WeekStatType.Pass_Sacked
		};

		/// <summary>
		/// Set of all the different rushing stat types.
		/// </summary>
		public static HashSet<WeekStatType> Rush = new HashSet<WeekStatType>
		{
			WeekStatType.Rush_Attempts,
			WeekStatType.Rush_Yards,
			WeekStatType.Rush_Touchdowns
		};

		/// <summary>
		/// Set of all the different receiving stat types.
		/// </summary>
		public static HashSet<WeekStatType> Receive = new HashSet<WeekStatType>
		{
			WeekStatType.Receive_Catches,
			WeekStatType.Receive_Yards,
			WeekStatType.Receive_Touchdowns
		};

		/// <summary>
		/// Set of all the different special-teams return stat types.
		/// </summary>
		public static HashSet<WeekStatType> Return = new HashSet<WeekStatType>
		{
			WeekStatType.Return_Yards,
			WeekStatType.Return_Touchdowns
		};

		/// <summary>
		/// Set of all the different misc stat types.
		/// </summary>
		public static HashSet<WeekStatType> Misc = new HashSet<WeekStatType>
		{
			WeekStatType.Fumble_Recover_Touchdowns,
			WeekStatType.Fumbles_Lost,
			WeekStatType.Fumbles_Total,
			WeekStatType.TwoPointConversions
		};

		/// <summary>
		/// Set of all the different kicking stat types.
		/// </summary>
		public static HashSet<WeekStatType> Kick = new HashSet<WeekStatType>
		{
			WeekStatType.Kick_PAT_Makes,
			WeekStatType.Kick_PAT_Misses,
			WeekStatType.Kick_ZeroTwenty_Makes,
			WeekStatType.Kick_TwentyThirty_Makes,
			WeekStatType.Kick_ThirtyForty_Makes,
			WeekStatType.Kick_FortyFifty_Makes,
			WeekStatType.Kick_FiftyPlus_Makes,
			WeekStatType.Kick_ZeroTwenty_Misses,
			WeekStatType.Kick_TwentyThirty_Misses,
			WeekStatType.Kick_ThirtyForty_Misses,
			WeekStatType.Kick_FortyFifty_Misses,
			WeekStatType.Kick_FiftyPlus_Misses
		};

		/// <summary>
		/// Set of all the different IDP stat types.
		/// </summary>
		public static HashSet<WeekStatType> IDP = new HashSet<WeekStatType>
		{
			WeekStatType.IDP_Tackles,
			WeekStatType.IDP_AssistedTackles,
			WeekStatType.IDP_Sacks,
			WeekStatType.IDP_Interceptions,
			WeekStatType.IDP_ForcedFumbles,
			WeekStatType.IDP_FumblesRecovered,
			WeekStatType.IDP_InterceptionTouchdowns,
			WeekStatType.IDP_FumbleTouchdowns,
			WeekStatType.IDP_BlockedKickTouchdowns,
			WeekStatType.IDP_BlockedKicks,
			WeekStatType.IDP_Safeties,
			WeekStatType.IDP_PassesDefended,
			WeekStatType.IDP_InterceptionReturnYards,
			WeekStatType.IDP_FumbleReturnYards,
			WeekStatType.IDP_TacklesForLoss,
			WeekStatType.IDP_QuarterBackHits,
			WeekStatType.IDP_SackYards
		};

		/// <summary>
		/// Set of all the different DST stat types.
		/// </summary>
		public static HashSet<WeekStatType> DST = new HashSet<WeekStatType>
		{
			WeekStatType.DST_Sacks,
			WeekStatType.DST_Interceptions,
			WeekStatType.DST_FumblesRecovered,
			WeekStatType.DST_FumblesForced,
			WeekStatType.DST_Safeties,
			WeekStatType.DST_Touchdowns,
			WeekStatType.DST_BlockedKicks,
			WeekStatType.DST_ReturnYards,
			WeekStatType.DST_ReturnTouchdowns,
			WeekStatType.DST_PointsAllowed,
			WeekStatType.DST_YardsAllowed
		};
	}
}
