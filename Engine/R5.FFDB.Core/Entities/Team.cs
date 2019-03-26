using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Entities
{
	/// <summary>
	/// Represents a single NFL team.
	/// </summary>
	public class Team
	{
		/// <summary>
		/// The id representing the team that's primarily used by the engine.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// The official NFL id of the team.
		/// </summary>
		public string NflId { get; set; }

		/// <summary>
		/// The official full name of the team (eg "Seattle Seahawks").
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The short name of the game (mainly used for URL mappings)
		/// </summary>
		public string ShortName { get; set; }

		/// <summary>
		/// The official abbreviation for the team (eg "SEA")
		/// </summary>
		public string Abbreviation { get; set; }

		/// <summary>
		/// Any known past abbreviations the team had used (eg "SD" for chargers, while current is "LAC")
		/// </summary>
		public HashSet<string> PriorAbbreviations { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		public override string ToString()
		{
			return Abbreviation;
		}
	}
}
