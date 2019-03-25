using R5.FFDB.Core.Models;
using System.Collections.Generic;

namespace R5.FFDB.Core.Entities
{
	/// <summary>
	/// Represents a set of stats for a single team's game.
	/// </summary>
	public class TeamWeekStats
	{
		/// <summary>
		/// The team's id.
		/// </summary>
		public int TeamId { get; set; }

		/// <summary>
		/// The week the game was played.
		/// </summary>
		public WeekInfo Week { get; set; }

		/// <summary>
		/// The complete list of player's NFL ids that played in this game.
		/// </summary>
		public List<string> PlayerNflIds { get; set; }

		/// <summary>
		/// Total points scored in the first quarter.
		/// </summary>
		public int PointsFirstQuarter { get; set; }

		/// <summary>
		/// Total points scored in the second quarter.
		/// </summary>
		public int PointsSecondQuarter { get; set; }

		/// <summary>
		/// Total points scored in the third quarter.
		/// </summary>
		public int PointsThirdQuarter { get; set; }

		/// <summary>
		/// Total points scored in the fourth quarter.
		/// </summary>
		public int PointsFourthQuarter { get; set; }

		/// <summary>
		/// Total points scored in the overtime period.
		/// </summary>
		public int PointsOverTime { get; set; }

		/// <summary>
		/// Total points scored in the entire game.
		/// </summary>
		public int PointsTotal { get; set; }

		/// <summary>
		/// The number of first downs gained.
		/// </summary>
		public int FirstDowns { get; set; }

		/// <summary>
		/// The total number of offensive yards.
		/// </summary>
		public int TotalYards { get; set; }

		/// <summary>
		/// The total number of passing yards.
		/// </summary>
		public int PassingYards { get; set; }

		/// <summary>
		/// The total number of rushing yards.
		/// </summary>
		public int RushingYards { get; set; }

		/// <summary>
		/// The total number of penalties called on the team.
		/// </summary>
		public int Penalties { get; set; }
		
		/// <summary>
		/// The total number of yards given up as a result of penalties.
		/// </summary>
		public int PenaltyYards { get; set; }

		/// <summary>
		/// The total number of turnovers committed by the team.
		/// </summary>
		public int Turnovers { get; set; }

		/// <summary>
		/// The total number of punts in the game.
		/// </summary>
		public int Punts { get; set; }

		/// <summary>
		/// The total number of yards punted in the game.
		/// </summary>
		public int PuntYards { get; set; }

		/// <summary>
		/// The total time of possession in the game.
		/// </summary>
		public int TimeOfPossessionSeconds { get; set; }

		public override string ToString()
		{
			return $"{TeamId} | {Week}";
		}
	}
}
