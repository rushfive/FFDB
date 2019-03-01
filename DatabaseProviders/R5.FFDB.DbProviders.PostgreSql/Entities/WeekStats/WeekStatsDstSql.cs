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
	[Table(Table.WeekStats.DST)]
	[CompositePrimaryKeys("team_id", "season", "week")]
	public class WeekStatsDstSql : WeekStatsSql
	{
		[NotNull]
		[ForeignKey(typeof(TeamSql), "id")]
		[Column("team_id", PostgresDataType.INT)]
		public int TeamId { get; set; }

		[NotNull]
		[Column("season", PostgresDataType.INT)]
		public override int Season { get; set; }

		[NotNull]
		[Column("week", PostgresDataType.INT)]
		public override int Week { get; set; }

		[WeekStatColumn("sacks", WeekStatType.DST_Sacks)]
		public double? Sacks { get; set; }

		[WeekStatColumn("interceptions", WeekStatType.DST_Interceptions)]
		public double? Interceptions { get; set; }

		[WeekStatColumn("fumbles_recovered", WeekStatType.DST_FumblesRecovered)]
		public double? FumblesRecovered { get; set; }

		[WeekStatColumn("fumbles_forced", WeekStatType.DST_FumblesForced)]
		public double? FumblesForced { get; set; }

		[WeekStatColumn("safeties", WeekStatType.DST_Safeties)]
		public double? Safeties { get; set; }

		[WeekStatColumn("touchdowns", WeekStatType.DST_Touchdowns)]
		public double? Touchdowns { get; set; }

		[WeekStatColumn("blocked_kicks", WeekStatType.DST_BlockedKicks)]
		public double? BlockedKicks { get; set; }

		[WeekStatColumn("return_yards", WeekStatType.DST_ReturnYards)]
		public double? ReturnYards { get; set; }

		[WeekStatColumn("return_touchdowns", WeekStatType.DST_ReturnTouchdowns)]
		public double? ReturnTouchdowns { get; set; }

		[WeekStatColumn("points_allowed", WeekStatType.DST_PointsAllowed)]
		public double? PointsAllowed { get; set; }

		[WeekStatColumn("yards_allowed", WeekStatType.DST_YardsAllowed)]
		public double? YardsAllowed { get; set; }

		public static WeekStatsDstSql FromCoreEntity(int teamId, WeekInfo week,
			IEnumerable<KeyValuePair<WeekStatType, double>> stats)
		{
			var result = new WeekStatsDstSql
			{
				TeamId = teamId,
				Season = week.Season,
				Week = week.Week
			};

			foreach (var kv in stats)
			{
				//WeekStatColumn column = EntityMetadata.GetWeekStatColumnByType(kv.Key);
				//column.SetValue(result, kv.Value);

				//PropertyInfo property = EntityMetadata.GetPropertyByStat(kv.Key);
				//property.SetValue(result, kv.Value);
			}

			return result;
		}

		public static IEnumerable<KeyValuePair<WeekStatType, double>> FilterStatValues(PlayerWeekStats stats)
		{
			return stats.Stats.Where(kv => WeekStatCategory.DST.Contains(kv.Key));
		}

		public override string PrimaryKeyMatchCondition()
		{
			return $"team_id = {TeamId} AND season = {Season} AND week = {Week}";
		}
	}
}
