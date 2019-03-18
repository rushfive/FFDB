using R5.FFDB.Core;
using R5.FFDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Entities.WeekStats
{
	public static class WeekStatsSqlExtensions
	{
		public static bool TryGetPassSql(this PlayerWeekStats stats,
			Dictionary<string, Guid> nflIdMap, out WeekStatsPassSql sql)
		{
			sql = null;

			var passingStats = stats.GetPassingStats();
			if (!passingStats.Any() || !nflIdMap.TryGetValue(stats.NflId, out Guid id))
			{
				return false;
			}

			sql = new WeekStatsPassSql
			{
				PlayerId = id,
				TeamId = stats.TeamId,
				Season = stats.Week.Season,
				Week = stats.Week.Week
			};

			sql.UpdateFromStats(passingStats);
			return true;
		}

		public static bool TryGetRushSql(this PlayerWeekStats stats,
			Dictionary<string, Guid> nflIdMap, out WeekStatsRushSql sql)
		{
			sql = null;

			var rushingStats = stats.GetRushingStats();
			if (!rushingStats.Any() || !nflIdMap.TryGetValue(stats.NflId, out Guid id))
			{
				return false;
			}

			sql = new WeekStatsRushSql
			{
				PlayerId = id,
				TeamId = stats.TeamId,
				Season = stats.Week.Season,
				Week = stats.Week.Week
			};

			sql.UpdateFromStats(rushingStats);
			return true;
		}

		public static bool TryGetReceiveSql(this PlayerWeekStats stats,
			Dictionary<string, Guid> nflIdMap, out WeekStatsReceiveSql sql)
		{
			sql = null;

			var receivingStats = stats.GetReceivingStats();
			if (!receivingStats.Any() || !nflIdMap.TryGetValue(stats.NflId, out Guid id))
			{
				return false;
			}

			sql = new WeekStatsReceiveSql
			{
				PlayerId = id,
				TeamId = stats.TeamId,
				Season = stats.Week.Season,
				Week = stats.Week.Week
			};

			sql.UpdateFromStats(receivingStats);
			return true;
		}

		public static bool TryGetReturnSql(this PlayerWeekStats stats,
			Dictionary<string, Guid> nflIdMap, out WeekStatsReturnSql sql)
		{
			sql = null;

			var returnStats = stats.GetReturnStats();
			if (!returnStats.Any() || !nflIdMap.TryGetValue(stats.NflId, out Guid id))
			{
				return false;
			}

			sql = new WeekStatsReturnSql
			{
				PlayerId = id,
				TeamId = stats.TeamId,
				Season = stats.Week.Season,
				Week = stats.Week.Week
			};

			sql.UpdateFromStats(returnStats);
			return true;
		}

		public static bool TryGetMiscSql(this PlayerWeekStats stats,
			Dictionary<string, Guid> nflIdMap, out WeekStatsMiscSql sql)
		{
			sql = null;

			var miscStats = stats.GetMiscStats();
			if (!miscStats.Any() || !nflIdMap.TryGetValue(stats.NflId, out Guid id))
			{
				return false;
			}

			sql = new WeekStatsMiscSql
			{
				PlayerId = id,
				TeamId = stats.TeamId,
				Season = stats.Week.Season,
				Week = stats.Week.Week
			};

			sql.UpdateFromStats(miscStats);
			return true;
		}

		public static bool TryGetKickSql(this PlayerWeekStats stats,
			Dictionary<string, Guid> nflIdMap, out WeekStatsKickSql sql)
		{
			sql = null;

			var kickStats = stats.GetKickingStats();
			if (!kickStats.Any() || !nflIdMap.TryGetValue(stats.NflId, out Guid id))
			{
				return false;
			}

			sql = new WeekStatsKickSql
			{
				PlayerId = id,
				TeamId = stats.TeamId,
				Season = stats.Week.Season,
				Week = stats.Week.Week
			};

			sql.UpdateFromStats(kickStats);
			return true;
		}

		public static bool TryGetIdpSql(this PlayerWeekStats stats,
			Dictionary<string, Guid> nflIdMap, out WeekStatsIdpSql sql)
		{
			sql = null;

			var idpStats = stats.GetIdpStats();
			if (!idpStats.Any() || !nflIdMap.TryGetValue(stats.NflId, out Guid id))
			{
				return false;
			}

			sql = new WeekStatsIdpSql
			{
				PlayerId = id,
				TeamId = stats.TeamId,
				Season = stats.Week.Season,
				Week = stats.Week.Week
			};

			sql.UpdateFromStats(idpStats);
			return true;
		}

		public static bool TryGetDstSql(this PlayerWeekStats stats,
			Dictionary<string, Guid> nflIdMap, out WeekStatsDstSql sql)
		{
			sql = null;

			var dstStats = stats.GetDstStats();
			if (!dstStats.Any() || !nflIdMap.TryGetValue(stats.NflId, out Guid id))
			{
				return false;
			}

			int teamId = TeamDataStore.GetIdFromNflId(stats.NflId);

			sql = new WeekStatsDstSql
			{
				TeamId = teamId,
				Season = stats.Week.Season,
				Week = stats.Week.Week
			};

			sql.UpdateFromStats(dstStats);
			return true;
		}
	}
}
