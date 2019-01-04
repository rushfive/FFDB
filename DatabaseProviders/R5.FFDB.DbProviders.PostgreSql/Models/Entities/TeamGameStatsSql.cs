using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models.Entities
{
	[TableName(Table.TeamGameStats)]
	[CompositePrimaryKeys("team_id", "season", "week")]
	public class TeamGameStatsSql : SqlEntity
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

		[NotNull]
		[Column("is_home_team", PostgresDataType.BOOLEAN)]
		public bool IsHomeTeam { get; set; }

		[NotNull]
		[Column("points_first_quarter", PostgresDataType.INT)]
		public int PointsFirstQuarter { get; set; }

		[NotNull]
		[Column("points_second_quarter", PostgresDataType.INT)]
		public int PointsSecondQuarter { get; set; }

		[NotNull]
		[Column("points_third_quarter", PostgresDataType.INT)]
		public int PointsThirdQuarter { get; set; }

		[NotNull]
		[Column("points_fourth_quarter", PostgresDataType.INT)]
		public int PointsFourthQuarter { get; set; }

		[NotNull]
		[Column("points_overtime", PostgresDataType.INT)]
		public int PointsOverTime { get; set; }

		[NotNull]
		[Column("points_total", PostgresDataType.INT)]
		public int PointsTotal { get; set; }

		[NotNull]
		[Column("first_downs", PostgresDataType.INT)]
		public int FirstDowns { get; set; }

		[NotNull]
		[Column("total_yards", PostgresDataType.INT)]
		public int TotalYards { get; set; }

		[NotNull]
		[Column("passing_yards", PostgresDataType.INT)]
		public int PassingYards { get; set; }

		[NotNull]
		[Column("rushing_yards", PostgresDataType.INT)]
		public int RushingYards { get; set; }

		[NotNull]
		[Column("penalties", PostgresDataType.INT)]
		public int Penalties { get; set; }

		[NotNull]
		[Column("penalty_yards", PostgresDataType.INT)]
		public int PenaltyYards { get; set; }

		[NotNull]
		[Column("turnovers", PostgresDataType.INT)]
		public int Turnovers { get; set; }

		[NotNull]
		[Column("punts", PostgresDataType.INT)]
		public int Punts { get; set; }

		[NotNull]
		[Column("punt_yards", PostgresDataType.INT)]
		public int PuntYards { get; set; }

		[NotNull]
		[Column("time_of_posession", PostgresDataType.INT)]
		public int TimeOfPossessionSeconds { get; set; }

		public static TeamGameStatsSql FromCoreEntity(TeamWeekStats stats)
		{
			return new TeamGameStatsSql
			{
				TeamId = stats.TeamId,
				Season = stats.Week.Season,
				Week = stats.Week.Week,
				IsHomeTeam = stats.IsHomeTeam,
				PointsFirstQuarter = stats.PointsFirstQuarter,
				PointsSecondQuarter = stats.PointsSecondQuarter,
				PointsThirdQuarter = stats.PointsThirdQuarter,
				PointsFourthQuarter = stats.PointsFourthQuarter,
				PointsOverTime = stats.PointsOverTime,
				PointsTotal = stats.PointsTotal,
				FirstDowns = stats.FirstDowns,
				TotalYards = stats.TotalYards,
				PassingYards = stats.PassingYards,
				RushingYards = stats.RushingYards,
				Penalties = stats.Penalties,
				PenaltyYards = stats.PenaltyYards,
				Turnovers = stats.Turnovers,
				Punts = stats.Punts,
				PuntYards = stats.PuntYards,
				TimeOfPossessionSeconds = stats.TimeOfPossessionSeconds
			};
		}
	}
}
