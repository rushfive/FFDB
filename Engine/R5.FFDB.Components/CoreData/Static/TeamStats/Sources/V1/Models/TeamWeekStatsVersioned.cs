using Newtonsoft.Json;
using R5.FFDB.Core.Models;
using System.Collections.Generic;

namespace R5.FFDB.Components.CoreData.Static.TeamStats.Sources.V1.Models
{
	public class TeamWeekStatsVersioned
	{
		[JsonProperty("week")]
		public WeekInfo Week { get; set; }

		[JsonProperty("homeTeamStats")]
		public Stats HomeTeamStats { get; set; }

		[JsonProperty("awayTeamStats")]
		public Stats AwayTeamStats { get; set; }

		public class Stats
		{
			[JsonProperty("id")]
			public int Id { get; set; }

			// list of players from team active, must map from esb to nflid
			// todo: better way of doing this, the DbContext providers shouldn't have
			// this info, its only used to populate the cache
			[JsonProperty("playerNflIds")]
			public List<string> PlayerNflIds { get; set; }

			[JsonProperty("pointsFirstQuarter")]
			public int PointsFirstQuarter { get; set; }

			[JsonProperty("pointsSecondQuarter")]
			public int PointsSecondQuarter { get; set; }

			[JsonProperty("pointsThirdQuarter")]
			public int PointsThirdQuarter { get; set; }

			[JsonProperty("pointsFourthQuarter")]
			public int PointsFourthQuarter { get; set; }

			[JsonProperty("pointsOverTime")]
			public int PointsOverTime { get; set; }

			[JsonProperty("pointsTotal")]
			public int PointsTotal { get; set; }


			[JsonProperty("firstDowns")]
			public int FirstDowns { get; set; }

			[JsonProperty("totalYards")]
			public int TotalYards { get; set; }

			[JsonProperty("passingYards")]
			public int PassingYards { get; set; }

			[JsonProperty("rushingYards")]
			public int RushingYards { get; set; }

			[JsonProperty("penalties")]
			public int Penalties { get; set; }

			[JsonProperty("penaltyYards")]
			public int PenaltyYards { get; set; }

			[JsonProperty("turnovers")]
			public int Turnovers { get; set; }

			[JsonProperty("punts")]
			public int Punts { get; set; }

			[JsonProperty("puntYards")]
			public int PuntYards { get; set; }

			[JsonProperty("timeOfPossessionSeconds")]
			public int TimeOfPossessionSeconds { get; set; }
		}
	}
}
