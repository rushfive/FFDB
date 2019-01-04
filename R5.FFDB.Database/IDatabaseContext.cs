using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Database
{
	// defines the required API contract for FFDB to work with
	// any database. 
	// The actual methods are grouped further into sub interfaces
	// based on domain/category
	public interface IDatabaseContext
	{
		ITeamDatabaseContext Team { get; }
		IPlayerDatabaseContext Player { get; }
		IWeekStatsDatabaseContext Stats { get; }
		Task InitializeAsync();
	}

	public interface ITeamDatabaseContext
	{
		Task AddTeamsAsync();
		Task UpdateRostersAsync(List<Roster> rosters);
		Task UpdateGameStatsAsync(List<TeamWeekStats> stats);
		
		// todo/roadmap
		//Task UpdateDepthChartsAsync();
		//Task UpdateStaticResourcesAsync(); // pictures, logos, etc
	}

	public interface IPlayerDatabaseContext
	{
		Task AddAsync(List<PlayerProfile> players, List<Roster> rosters,
			int insertBatchCount = 500);

		Task<List<PlayerProfile>> GetAllAsync();

		//todo remove nd use getallasync
		Task<List<string>> GetExistingNflIdsAsync();

		Task UpdateAsync(List<PlayerProfile> players, bool overrideExisting);
	}

	public interface IWeekStatsDatabaseContext
	{
		Task<List<WeekInfo>> GetExistingWeeksAsync();
		Task UpdateWeeksAsync(List<WeekStats> stats);
	}

	// should be a class that configures and returns the 
	// db context. The provider handles accepting configuration,
	// making the connection to the db, etc. the databasecontext
	// implementation should simply take in the connection as 
	// a constructor param and use it
	public interface IDatabaseProvider
	{
		IDatabaseContext GetContext();
	}
}
