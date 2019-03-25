namespace R5.FFDB.Core.Database
{
	/// <summary>
	/// Represents the contract type the Engine requires to interface with a given database.
	/// </summary>
	public interface IDatabaseProvider
	{
		/// <summary>
		/// Returns the database context the Engine uses to make database calls and updates.
		/// </summary>
		IDatabaseContext GetContext();
	}
}
