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
	public abstract class WeekStatsPlayerSql : WeekStatsSql
	{
		public abstract Guid PlayerId { get; set; }

		// Can be null, as safety in case we can't resolve it from sources
		public abstract int? TeamId { get; set; }

		// returns a list because each player can have different week stat 
		// entry types (eg scored a rushing td AND made a 50+ kick)
		public static List<WeekStatsPlayerSql> FromCoreEntity(PlayerWeekStats stats,
			Guid playerId, WeekInfo week)
		{
			var result = new List<WeekStatsPlayerSql>();

			var passStats = stats.Stats.Where(kv => WeekStatCategory.Pass.Contains(kv.Key));
			var rushStats = stats.Stats.Where(kv => WeekStatCategory.Rush.Contains(kv.Key));
			var receiveStats = stats.Stats.Where(kv => WeekStatCategory.Receive.Contains(kv.Key));
			var returnStats = stats.Stats.Where(kv => WeekStatCategory.Return.Contains(kv.Key));
			var miscStats = stats.Stats.Where(kv => WeekStatCategory.Misc.Contains(kv.Key));
			var kickStats = stats.Stats.Where(kv => WeekStatCategory.Kick.Contains(kv.Key));
			var idpStats = stats.Stats.Where(kv => WeekStatCategory.IDP.Contains(kv.Key));

			if (passStats.Any())
			{
				addWeekStatSql(new WeekStatsPassSql(), passStats);
			}
			if (rushStats.Any())
			{
				addWeekStatSql(new WeekStatsRushSql(), rushStats);
			}
			if (receiveStats.Any())
			{
				addWeekStatSql(new WeekStatsReceiveSql(), receiveStats);
			}
			if (returnStats.Any())
			{
				addWeekStatSql(new WeekStatsReturnSql(), returnStats);
			}
			if (miscStats.Any())
			{
				addWeekStatSql(new WeekStatsMiscSql(), miscStats);
			}
			if (kickStats.Any())
			{
				addWeekStatSql(new WeekStatsKickSql(), kickStats);
			}
			if (idpStats.Any())
			{
				addWeekStatSql(new WeekStatsIdpSql(), idpStats);
			}

			return result;

			// local functions
			void addWeekStatSql(WeekStatsPlayerSql statsSql,
				IEnumerable<KeyValuePair<WeekStatType, double>> statValues)
			{
				statsSql.PlayerId = playerId;
				statsSql.TeamId = stats.TeamId;
				statsSql.Season = week.Season;
				statsSql.Week = week.Week;

				foreach (var kv in statValues)
				{
					//WeekStatColumn column = EntityMetadata.GetWeekStatColumnByType(kv.Key);
					//column.SetValue(statsSql, kv.Value);

					//PropertyInfo property = EntityMetadata.GetPropertyByStat(kv.Key);
					//property.SetValue(statsSql, kv.Value);
				}

				result.Add(statsSql);
			}
		}

		
	}
}
