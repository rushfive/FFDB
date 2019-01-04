using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities.WeekStats;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models
{
	public class WeekStatsSqlUpdate
	{
		public WeekInfo Week { get; set; }
		public List<WeekStatsPassSql> PassStats { get; } = new List<WeekStatsPassSql>();
		public List<WeekStatsRushSql> RushStats { get; } = new List<WeekStatsRushSql>();
		public List<WeekStatsReceiveSql> ReceiveStats { get; } = new List<WeekStatsReceiveSql>();
		public List<WeekStatsMiscSql> MiscStats { get; } = new List<WeekStatsMiscSql>();
		public List<WeekStatsKickSql> KickStats { get; } = new List<WeekStatsKickSql>();
		public List<WeekStatsDstSql> DstStats { get; } = new List<WeekStatsDstSql>();
		public List<WeekStatsIdpSql> IdpStats { get; } = new List<WeekStatsIdpSql>();
	}
}
