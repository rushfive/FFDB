using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Entities
{
	/// <summary>
	/// Represents the current roster information for a given team.
	/// </summary>
	public class Roster
	{
		/// <summary>
		/// The team's id.
		/// </summary>
		public int TeamId { get; set; }

		/// <summary>
		/// The team's abbreviation.
		/// </summary>
		public string TeamAbbreviation { get; set; }

		/// <summary>
		/// The list of players currently rostered on the team.
		/// </summary>
		public List<RosterPlayer> Players { get; set; }

		public override string ToString()
		{
			return $"{TeamAbbreviation} Roster";
		}
	}

	/// <summary>
	/// Represents a player that's currently rostered.
	/// </summary>
	public class RosterPlayer
	{
		/// <summary>
		/// The official NFL player id.
		/// </summary>
		public string NflId { get; set; }

		/// <summary>
		/// The player's current team number.
		/// </summary>
		public int? Number { get; set; }

		/// <summary>
		/// The player's current position type.
		/// </summary>
		public Position Position { get; set; }

		/// <summary>
		/// The player's current roster status type.
		/// </summary>
		public RosterStatus Status { get; set; }
	}
}
