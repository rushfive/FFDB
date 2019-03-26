using R5.FFDB.Core.Models;

namespace R5.FFDB.Core.Entities
{
	/// <summary>
	/// Represents the update information for a player.
	/// Note that most of a player's profile data points are static and never change.
	/// This model represents some of the dynamic properties.
	/// </summary>
	public class PlayerUpdate
	{
		/// <summary>
		/// The player's current team number.
		/// </summary>
		public int? Number { get; set; }

		/// <summary>
		/// The player's current position.
		/// </summary>
		public Position? Position { get; set; }

		/// <summary>
		/// The player's current roster status.
		/// </summary>
		public RosterStatus? Status { get; set; }
	}
}
