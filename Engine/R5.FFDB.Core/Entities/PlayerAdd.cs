using System;

namespace R5.FFDB.Core.Entities
{
	/// <summary>
	/// Represents the data required to add a player.
	/// This is a combination of static and dynamic data points.
	/// </summary>
	public class PlayerAdd : PlayerUpdate
	{
		/// <summary>
		/// The player's first name.
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// The player's last name.
		/// </summary>
		public string LastName { get; set; }

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
		/// The player's official draft profile height.
		/// </summary>
		public int Height { get; set; }

		/// <summary>
		/// The player's official draft profile weight.
		/// </summary>
		public int Weight { get; set; }

		/// <summary>
		/// The player's date of birth.
		/// </summary>
		public DateTimeOffset DateOfBirth { get; set; }

		/// <summary>
		/// The college the player attended (if any)
		/// </summary>
		public string College { get; set; }

		public override string ToString()
		{
			string name = base.ToString();
			return $"{NflId} ({name})";
		}
	}
}
