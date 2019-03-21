using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R5.FFDB.Core.Database
{
	public interface IDatabaseContext
	{
		Task InitializeAsync();
		Task<bool> HasBeenInitializedAsync();

		IPlayerDbContext Player { get; }
		IPlayerStatsDbContext PlayerStats { get; }
		ITeamDbContext Team { get; }
		ITeamStatsDbContext TeamStats { get; }
		IUpdateLogDbContext UpdateLog { get; }
		IWeekMatchupsDbContext WeekMatchups { get; }
	}

	public interface IPlayerDbContext
	{
		Task<List<Player>> GetAllAsync();
		Task AddAsync(PlayerAdd player);
		Task UpdateAsync(Guid id, PlayerUpdate update);
	}

	public interface IPlayerStatsDbContext
	{
		Task<List<string>> GetPlayerNflIdsAsync(WeekInfo week);
		Task AddAsync(List<PlayerWeekStats> stats);
	}

	public interface ITeamDbContext
	{
		Task AddAsync(List<Team> teams);
		Task UpdateRosterMappingsAsync(List<Roster> rosters);
	}

	public interface ITeamStatsDbContext
	{
		Task<List<TeamWeekStats>> GetAsync(WeekInfo week);
		Task AddAsync(List<TeamWeekStats> stats);
	}

	public interface IUpdateLogDbContext
	{
		Task<List<WeekInfo>> GetAsync();
		Task AddAsync(WeekInfo week);
		Task<bool> HasUpdatedWeekAsync(WeekInfo week);
	}

	public interface IWeekMatchupsDbContext
	{
		Task<List<WeekMatchup>> GetAsync(WeekInfo week);
		Task AddAsync(List<WeekMatchup> matchups);
	}
}
