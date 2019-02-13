using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R5.FFDB.Core.Database.DbContext
{
	public interface IDatabaseContext
	{
		//ITeamDatabaseContext Team { get; }
		//IPlayerDatabaseContext Player { get; }
		//IWeekStatsDatabaseContext Stats { get; }
		//ILogDatabaseContext Log { get; }

		IPlayerDbContext Player { get; }
		ITeamDbContext Team { get; }
		IWeekMatchupsDbContext WeekMatchups { get; }
		IPlayerStatsDbContext PlayerStats { get; }
		ITeamStatsDbContext TeamStats { get; }
		IUpdateLogDbContext UpdateLog { get; }


		Task<bool> HasBeenInitializedAsync();

		// force allows re-initialization, by first clearing all existing 
		// ffdb then re-running normal init routine.
		Task InitializeAsync(bool force);
	}

	public interface IPlayerDbContext
	{
		Task<List<Player>> GetAllAsync();
		Task AddAsync(PlayerAdd player);
		Task UpdateAsync(Guid id, PlayerUpdate update);
	}

	public interface ITeamDbContext
	{
		Task AddAllAsync(List<Team> teams);
		Task UpdateRosterMappingsAsync(List<Roster> rosters);
	}

	public interface IWeekMatchupsDbContext
	{
		Task AddAsync(List<WeekMatchup> matchups);
		Task<List<WeekMatchup>> GetAsync(WeekInfo week);
	}
	
	public interface IPlayerStatsDbContext
	{
		Task AddAsync(List<PlayerWeekStats> stats);
		Task<List<PlayerWeekStats>> GetAsync(WeekInfo week);
	}

	public interface ITeamStatsDbContext
	{
		Task AddAsync(List<TeamWeekStats> stats);
		Task<List<TeamWeekStats>> GetAsync(WeekInfo week);
	}

	public interface IUpdateLogDbContext
	{
		Task<List<WeekInfo>> GetAsync();
		Task<bool> HasUpdatedWeekAsync(WeekInfo week);
		Task AddAsync(WeekInfo week);
	}
}
