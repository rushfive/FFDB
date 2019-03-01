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
	[Table(TableName.WeekStats.Kick)]
	[CompositePrimaryKeys("player_id", "season", "week")]
	public class WeekStatsKickSql : WeekStatsPlayerSql
	{
		[NotNull]
		[ForeignKey(typeof(PlayerSql), "id")]
		[Column("player_id", PostgresDataType.UUID)]
		public override Guid PlayerId { get; set; }
		
		[ForeignKey(typeof(TeamSql), "id")]
		[Column("team_id", PostgresDataType.INT)]
		public override int? TeamId { get; set; }

		[NotNull]
		[Column("season", PostgresDataType.INT)]
		public override int Season { get; set; }

		[NotNull]
		[Column("week", PostgresDataType.INT)]
		public override int Week { get; set; }

		[WeekStatColumn("pat_makes", WeekStatType.Kick_PAT_Makes)]
		public double? PatMakes { get; set; }

		[WeekStatColumn("pat_misses", WeekStatType.Kick_PAT_Misses)]
		public double? PatMisses { get; set; }

		[WeekStatColumn("zero_twenty_makes", WeekStatType.Kick_ZeroTwenty_Makes)]
		public double? ZeroTwentyMakes { get; set; }

		[WeekStatColumn("twenty_thirty_makes", WeekStatType.Kick_TwentyThirty_Makes)]
		public double? TwentyThirtyMakes { get; set; }

		[WeekStatColumn("thirty_forty_makes", WeekStatType.Kick_ThirtyForty_Makes)]
		public double? ThirtyFortyMakes { get; set; }

		[WeekStatColumn("forty_fifty_makes", WeekStatType.Kick_FortyFifty_Makes)]
		public double? FortyFiftyMakes { get; set; }

		[WeekStatColumn("fifty_plus_makes", WeekStatType.Kick_FiftyPlus_Makes)]
		public double? FiftyPlusMakes { get; set; }

		[WeekStatColumn("zero_twenty_misses", WeekStatType.Kick_ZeroTwenty_Misses)]
		public double? ZeroTwentyMisses { get; set; }

		[WeekStatColumn("twenty_thirty_misses", WeekStatType.Kick_TwentyThirty_Misses)]
		public double? TwentyThirtyMisses { get; set; }

		[WeekStatColumn("thirty_forty_misses", WeekStatType.Kick_ThirtyForty_Misses)]
		public double? ThirtyFortyMisses { get; set; }

		[WeekStatColumn("forty_fifty_misses", WeekStatType.Kick_FortyFifty_Misses)]
		public double? FortyFiftyMisses { get; set; }

		[WeekStatColumn("fifty_plus_misses", WeekStatType.Kick_FiftyPlus_Misses)]
		public double? FiftyPlusMisses { get; set; }

		public override string PrimaryKeyMatchCondition()
		{
			return $"player_id = '{PlayerId}' AND season = {Season} AND week = {Week}";
		}
	}
}
