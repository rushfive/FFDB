using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Entities
{
	/// <summary>
	/// Represents the player information required by the Engine for various functionality.
	/// </summary>
	public class Player
	{
		/// <summary>
		/// The player's database id.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// The player's official NFL id.
		/// </summary>
		public string NflId { get; set; }

		/// <summary>
		/// The player's ESB id.
		/// </summary>
		public string EsbId { get; set; }

		/// <summary>
		/// The player's GSIS id.
		/// </summary>
		public string GsisId { get; set; }

		/// <summary>
		/// The player's first name.
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// The player's last name.
		/// </summary>
		public string LastName { get; set; }

		public override string ToString()
		{
			string name = $"{FirstName} {LastName}".Trim();
			return $"{NflId} ({name})";
		}
	}
}
