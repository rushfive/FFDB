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
	[Table(TableName.WeekStats.DST)]
	[CompositePrimaryKeys("team_id", "season", "week")]
	public class WeekStatsDstSql
	{
		[NotNull]
		[ForeignKey(typeof(TeamSql), "id")]
		[Column("team_id", PostgresDataType.INT)]
		public int TeamId { get; set; }

		[NotNull]
		[Column("season", PostgresDataType.INT)]
		public int Season { get; set; }

		[NotNull]
		[Column("week", PostgresDataType.INT)]
		public int Week { get; set; }

		[Column("sacks", PostgresDataType.FLOAT8)]
		public double? Sacks { get; set; }

		[Column("interceptions", PostgresDataType.FLOAT8)]
		public double? Interceptions { get; set; }

		[Column("fumbles_recovered", PostgresDataType.FLOAT8)]
		public double? FumblesRecovered { get; set; }

		[Column("fumbles_forced", PostgresDataType.FLOAT8)]
		public double? FumblesForced { get; set; }

		[Column("safeties", PostgresDataType.FLOAT8)]
		public double? Safeties { get; set; }

		[Column("touchdowns", PostgresDataType.FLOAT8)]
		public double? Touchdowns { get; set; }

		[Column("blocked_kicks", PostgresDataType.FLOAT8)]
		public double? BlockedKicks { get; set; }

		[Column("return_yards", PostgresDataType.FLOAT8)]
		public double? ReturnYards { get; set; }

		[Column("return_touchdowns", PostgresDataType.FLOAT8)]
		public double? ReturnTouchdowns { get; set; }

		[Column("points_allowed", PostgresDataType.FLOAT8)]
		public double? PointsAllowed { get; set; }

		[Column("yards_allowed", PostgresDataType.FLOAT8)]
		public double? YardsAllowed { get; set; }

		//public static WeekStatsDstSql FromCoreEntity(int teamId, WeekInfo week,
		//	IEnumerable<KeyValuePair<WeekStatType, double>> stats)
		//{
		//	var result = new WeekStatsDstSql
		//	{
		//		TeamId = teamId,
		//		Season = week.Season,
		//		Week = week.Week
		//	};

		//	foreach (var kv in stats)
		//	{
		//		//Column column = EntityMetadata.GetColumnByType(kv.Key);
		//		//column.SetValue(result, kv.Value);

		//		//PropertyInfo property = EntityMetadata.GetPropertyByStat(kv.Key);
		//		//property.SetValue(result, kv.Value);
		//	}

		//	return result;
		//}

		//public static IEnumerable<KeyValuePair<WeekStatType, double>> FilterStatValues(PlayerWeekStats stats)
		//{
		//	return stats.Stats.Where(kv => WeekStatCategory.DST.Contains(kv.Key));
		//}

		//public override string PrimaryKeyMatchCondition()
		//{
		//	return $"team_id = {TeamId} AND season = {Season} AND week = {Week}";
		//}
	}
}
