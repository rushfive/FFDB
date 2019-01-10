using System.Threading.Tasks;

namespace R5.FFDB.Database.DbContext
{
	public interface IDatabaseContext
	{
		ITeamDatabaseContext Team { get; }
		IPlayerDatabaseContext Player { get; }
		IWeekStatsDatabaseContext Stats { get; }
		ILogDatabaseContext Log { get; }

		Task InitializeAsync();
	}
}
