using System.Threading.Tasks;

namespace R5.FFDB.Database.DbContext
{
	public interface IDatabaseContext
	{
		ITeamDatabaseContext Team { get; }
		IPlayerDatabaseContext Player { get; }
		IWeekStatsDatabaseContext Stats { get; }
		ILogDatabaseContext Log { get; }

		Task<bool> HasBeenInitializedAsync();

		// force allows re-initialization, by first clearing all existing 
		// ffdb then re-running normal init routine.
		Task InitializeAsync(bool force);
	}
}
