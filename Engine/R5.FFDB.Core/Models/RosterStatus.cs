namespace R5.FFDB.Core.Models
{
	/// <summary>
	/// Represents a player's current roster status.
	/// </summary>
	public enum RosterStatus
	{
		/// <summary>
		/// Active
		/// </summary>
		ACT,

		/// <summary>
		/// Injured Reserve
		/// </summary>
		RES,

		/// <summary>
		/// Non football-related Injured Reserve
		/// </summary>
		NON,

		/// <summary>
		/// Suspended
		/// </summary>
		SUS,

		/// <summary>
		/// Physically Unable to Perform
		/// </summary>
		PUP,

		/// <summary>
		/// Unsigned Draft Pick
		/// </summary>
		UDF,

		/// <summary>
		/// Exempt
		/// </summary>
		EXE
	}
}
