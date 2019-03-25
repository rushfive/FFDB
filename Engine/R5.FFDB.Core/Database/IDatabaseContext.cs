using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R5.FFDB.Core.Database
{
	/// <summary>
	/// Represents the main (root) database context.
	/// It contains a couple methods, and is a container for other categorized contexts.
	/// </summary>
	public interface IDatabaseContext
	{
		/// <summary>
		/// Runs the initial database tasks (such as creating tables)
		/// </summary>
		Task InitializeAsync();

		/// <summary>
		/// Determines whether the database has been initialized or not.
		/// This is used to block database updates that first require initialization.
		/// </summary>
		Task<bool> HasBeenInitializedAsync();

		/// <summary>
		/// Database context containing player specific functions.
		/// </summary>
		IPlayerDbContext Player { get; }

		/// <summary>
		/// Database context containing player-stats specific functions.
		/// </summary>
		IPlayerStatsDbContext PlayerStats { get; }

		/// <summary>
		/// Database context containing team specific functions.
		/// </summary>
		ITeamDbContext Team { get; }

		/// <summary>
		/// Database context containing team-stats specific functions.
		/// </summary>
		ITeamStatsDbContext TeamStats { get; }

		/// <summary>
		/// Database context containing updated-logs specific functions.
		/// </summary>
		IUpdateLogDbContext UpdateLog { get; }

		/// <summary>
		/// Database context containing week-matchups specific functions.
		/// </summary>
		IWeekMatchupsDbContext WeekMatchups { get; }
	}

	/// <summary>
	/// Database context containing player specific functions.
	/// </summary>
	public interface IPlayerDbContext
	{
		/// <summary>
		/// Returns the list of all players currently existing in database.
		/// </summary>
		Task<List<Player>> GetAllAsync();

		/// <summary>
		/// Adds a given player.
		/// </summary>
		Task AddAsync(PlayerAdd player);

		/// <summary>
		/// Updates the player specified by their database id using the update model.
		/// </summary>
		Task UpdateAsync(Guid id, PlayerUpdate update);
	}

	/// <summary>
	/// Database context containing player-stats specific functions.
	/// </summary>
	public interface IPlayerStatsDbContext
	{
		/// <summary>
		/// Get the complete list of player's official NFL ids that have played for a given week.
		/// </summary>
		Task<List<string>> GetPlayerNflIdsAsync(WeekInfo week);

		/// <summary>
		/// Adds the list of player stats to the database.
		/// </summary>
		Task AddAsync(List<PlayerWeekStats> stats);
	}

	/// <summary>
	/// Database context containing team specific functions.
	/// </summary>
	public interface ITeamDbContext
	{
		/// <summary>
		/// Get the list of team ids that already exist in the database.
		/// </summary>
		Task<List<int>> GetExistingTeamIdsAsync();

		/// <summary>
		/// Add the list of teams to the database.
		/// </summary>
		Task AddAsync(List<Team> teams);

		/// <summary>
		/// Update the player-to-team mappings given a list of roster information.
		/// </summary>
		Task UpdateRosterMappingsAsync(List<Roster> rosters);
	}

	/// <summary>
	/// Database context containing team-stats specific functions.
	/// </summary>
	public interface ITeamStatsDbContext
	{
		/// <summary>
		/// Get a list of game stats for all teams for a specified week.
		/// </summary>
		Task<List<TeamWeekStats>> GetAsync(WeekInfo week);

		/// <summary>
		/// Adds a list of team's game stats to the database.
		/// </summary>
		Task AddAsync(List<TeamWeekStats> stats);
	}

	/// <summary>
	/// Database context containing updated-logs specific functions.
	/// </summary>
	public interface IUpdateLogDbContext
	{
		/// <summary>
		/// Get a complete list of all weeks that have already been updated in the database.
		/// </summary>
		Task<List<WeekInfo>> GetAsync();

		/// <summary>
		/// Update the database to indicate that the specified week had all required tasks completed.
		/// </summary>
		Task AddAsync(WeekInfo week);

		/// <summary>
		/// Determines whether the specified week has had all required tasks completed.
		/// </summary>
		Task<bool> HasUpdatedWeekAsync(WeekInfo week);
	}

	/// <summary>
	/// Database context containing week-matchups specific functions.
	/// </summary>
	public interface IWeekMatchupsDbContext
	{
		/// <summary>
		/// Get the complete list of team-vs-team matchups for a specified week.
		/// </summary>
		Task<List<WeekMatchup>> GetAsync(WeekInfo week);

		/// <summary>
		/// Add a list of matchups to the database.
		/// </summary>
		Task AddAsync(List<WeekMatchup> matchups);
	}
}
