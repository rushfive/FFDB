﻿using R5.FFDB.Core.Models;
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
		IStatsDatabaseContext Stats { get; }

		Task RunInitialSetupAsync();
	}

	public interface ITeamDatabaseContext
	{
		Task UpdateRostersAsync(List<Roster> rosters);
		
		// todo/roadmap
		//Task UpdateDepthChartsAsync();
		//Task UpdateStaticResourcesAsync(); // pictures, logos, etc
	}

	public interface IPlayerDatabaseContext
	{
		Task<List<PlayerProfile>> GetExistingAsync();
		Task UpdateAsync(List<PlayerProfile> players, bool overrideExisting);
	}

	public interface IStatsDatabaseContext
	{
		Task<List<WeekInfo>> GetExistingWeeksAsync();
		Task UpdateWeeksAsync(List<WeekStats> stats, bool overrideExisting);
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