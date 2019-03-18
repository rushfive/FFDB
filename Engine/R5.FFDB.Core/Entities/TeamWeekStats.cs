using R5.FFDB.Core.Models;
using System.Collections.Generic;

namespace R5.FFDB.Core.Entities
{
	public class TeamWeekStats
	{
		public int TeamId { get; set; }
		public WeekInfo Week { get; set; }

		// todo: create derived model that contains ths property
		// its only used in engine to populate a cache
		// dbContext doesnt care about this nor sohuld it
		public List<string> PlayerNflIds { get; set; }

		// points
		public int PointsFirstQuarter { get; set; }
		public int PointsSecondQuarter { get; set; }
		public int PointsThirdQuarter { get; set; }
		public int PointsFourthQuarter { get; set; }
		public int PointsOverTime { get; set; }
		public int PointsTotal { get; set; }

		// stats
		public int FirstDowns { get; set; }
		public int TotalYards { get; set; }
		public int PassingYards { get; set; }
		public int RushingYards { get; set; }
		public int Penalties { get; set; }
		public int PenaltyYards { get; set; }
		public int Turnovers { get; set; }
		public int Punts { get; set; }
		public int PuntYards { get; set; }
		public int TimeOfPossessionSeconds { get; set; }

		public override string ToString()
		{
			return $"{TeamId} | {Week}";
		}
	}
}
