using R5.FFDB.Core.Models;

namespace R5.FFDB.Core.Entities
{
	/// <summary>
	/// Represents a single specific game from a given week between two teams.
	/// </summary>
	public class WeekMatchup
	{
		/// <summary>
		/// The week the game was played.
		/// </summary>
		public WeekInfo Week { get; set; }

		/// <summary>
		/// The home team's id.
		/// </summary>
		public int HomeTeamId { get; set; }

		/// <summary>
		/// The away team's id.
		/// </summary>
		public int AwayTeamId { get; set; }

		/// <summary>
		/// The NFL's official id for this game.
		/// </summary>
		public string NflGameId { get; set; }

		/// <summary>
		/// The NFL's official GSIS id for this game.
		/// </summary>
		public string GsisGameId { get; set; }

		public override string ToString()
		{
			return $"{HomeTeamId} vs {AwayTeamId} ({Week})";
		}
	}
}
