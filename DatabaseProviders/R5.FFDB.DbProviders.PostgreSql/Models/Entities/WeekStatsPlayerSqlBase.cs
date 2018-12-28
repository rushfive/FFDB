using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models.Entities
{
	public abstract class WeekStatsPlayerSqlBase : WeekStatsSqlBase
	{
		public abstract Guid PlayerId { get; set; }
		public abstract int? TeamId { get; set; }

		// returns a list because each player can have different week stat 
		// entry types (eg scored a rushing td AND made a 50+ kick)
		public static List<WeekStatsPlayerSqlBase> FromCoreEntity(PlayerStats stats,
			Guid playerId, WeekInfo week)
		{
			var result = new List<WeekStatsPlayerSqlBase>();

			var weekStatValues = stats.Stats.Where(kv => _weekStatTypes.Contains(kv.Key));
			var weekStatKickerValues = stats.Stats.Where(kv => _weekStatKickerTypes.Contains(kv.Key));
			var weekStatIdpValues = stats.Stats.Where(kv => _weekStatIdpTypes.Contains(kv.Key));

			if (weekStatValues.Any())
			{
				addWeekStatSql(new WeekStatsSql(), weekStatValues);
			}
			if (weekStatKickerValues.Any())
			{
				addWeekStatSql(new WeekStatsKickerSql(), weekStatKickerValues);
			}
			if (weekStatIdpValues.Any())
			{
				addWeekStatSql(new WeekStatsIdpSql(), weekStatIdpValues);
			}

			return result;

			// local functions
			void addWeekStatSql(WeekStatsPlayerSqlBase statsSql,
				IEnumerable<KeyValuePair<WeekStatType, double>> statValues)
			{
				statsSql.PlayerId = playerId;
				statsSql.TeamId = stats.TeamId;
				statsSql.Season = week.Season;
				statsSql.Week = week.Week;

				foreach (var kv in statValues)
				{
					PropertyInfo property = EntityInfoMap.GetPropertyByStat(kv.Key);
					property.SetValue(statsSql, kv.Value);
				}

				result.Add(statsSql);
			}
		}

		private static HashSet<WeekStatType> _weekStatTypes = new HashSet<WeekStatType>
		{
			WeekStatType.Pass_Attempts,
			WeekStatType.Pass_Completions,
			WeekStatType.Pass_Yards,
			WeekStatType.Pass_Touchdowns,
			WeekStatType.Pass_Interceptions,
			WeekStatType.Pass_Sacked,

			WeekStatType.Rush_Attempts,
			WeekStatType.Rush_Yards,
			WeekStatType.Rush_Touchdowns,

			WeekStatType.Receive_Catches,
			WeekStatType.Receive_Yards,
			WeekStatType.Receive_Touchdowns,

			WeekStatType.Return_Yards,
			WeekStatType.Return_Touchdowns,

			WeekStatType.Fumble_Recover_Touchdowns,
			WeekStatType.Fumbles_Lost,
			WeekStatType.Fumbles_Total,

			WeekStatType.TwoPointConversions
		};

		private static HashSet<WeekStatType> _weekStatKickerTypes = new HashSet<WeekStatType>
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

		private static HashSet<WeekStatType> _weekStatIdpTypes = new HashSet<WeekStatType>
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
	}
}
