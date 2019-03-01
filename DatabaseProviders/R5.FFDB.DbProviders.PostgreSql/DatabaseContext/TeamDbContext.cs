//using Microsoft.Extensions.Logging;
//using Npgsql;
//using R5.FFDB.Core;
//using R5.FFDB.Core.Database;
//using R5.FFDB.Core.Entities;
//using R5.FFDB.Core.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
//{
//	public class TeamDbContext : DbContextBase, ITeamDbContext
//	{
//		public TeamDbContext(
//			Func<NpgsqlConnection> getConnection,
//			ILoggerFactory loggerFactory)
//			: base(getConnection, loggerFactory)
//		{
//		}

//		public async Task AddAsync(List<Team> teams)
//		{
//			if (teams == null)
//			{
//				throw new ArgumentNullException(nameof(teams), "Teams must be provided.");
//			}

//			ILogger<TeamDbContext> logger = GetLogger<TeamDbContext>();
//			PostgresDbContext db = GetPostgresDbContext();

//			HashSet<int> existing = (await db.SelectAsync<TeamSql>())
//				.Select(t => t.Id)
//				.ToHashSet();

//			List<TeamSql> missing = teams
//				.Where(t => !existing.Contains(t.Id))
//				.Select(TeamSql.FromCoreEntity)
//				.ToList();

//			if (!missing.Any())
//			{
//				return;
//			}

//			string insertSql = SqlCommandBuilder.Rows.InsertMany(missing);

//			await db.ExecuteCommandAsync(insertSql);

//			logger.LogTrace($"Added {missing.Count} teams to '{EntityMetadata.TableName<TeamSql>()}' table.");
//		}

//		public async Task UpdateRosterMappingsAsync(List<Roster> rosters)
//		{
//			if (rosters == null)
//			{
//				throw new ArgumentNullException(nameof(rosters), "Rosters must be provided.");
//			}

//			ILogger<TeamDbContext> logger = GetLogger<TeamDbContext>();
//			PostgresDbContext db = GetPostgresDbContext();

//			logger.LogTrace($"Updating roster mappings..");
			
//			await db.TruncateAsync<PlayerTeamMapSql>();

//			Dictionary<string, Guid> nflIdMap = await GetNflIdMapAsync(db);

//			foreach (Roster roster in rosters)
//			{
//				await UpdateForRosterAsync(roster, nflIdMap, db);
//			}

//			logger.LogTrace($"Updated roster mappings for players in '{EntityMetadata.TableName<PlayerTeamMapSql>()}' table.");
//		}

//		private async Task<Dictionary<string, Guid>> GetNflIdMapAsync(PostgresDbContext db)
//		{
//			string playerTableName = EntityMetadata.TableName(typeof(PlayerSql));

//			List<PlayerSql> players = await db.SelectAsync<PlayerSql>($"SELECT id, nfl_id FROM {playerTableName};");
//			return players.ToDictionary(p => p.NflId, p => p.Id);
//		}

//		private Task UpdateForRosterAsync(Roster roster,
//			Dictionary<string, Guid> nflIdMap, PostgresDbContext db)
//		{
//			var entries = roster.Players
//				.Where(p => nflIdMap.ContainsKey(p.NflId))
//				.Select(p => PlayerTeamMapSql.ToSqlEntity(
//					nflIdMap[p.NflId], roster.TeamId));

//			return db.InsertManyAsync(entries);
//		}

//		//public async Task AddTeamsAsync()
//		//{
//		//	string tableName = EntityMetadata.TableName(typeof(TeamSql));
//		//	ILogger<TeamDbContext> logger = GetLogger<TeamDbContext>();

//		//	logger.LogDebug($"Adding NFL team entries to '{tableName}' table.");

//		//	IEnumerable<TeamSql> teamSqls = TeamDataStore
//		//		.GetAll()
//		//		.Select(TeamSql.FromCoreEntity);

//		//	string insertTeamsSql = SqlCommandBuilder.Rows.InsertMany(teamSqls);

//		//	logger.LogTrace("Adding teams using SQL command: " + Environment.NewLine + insertTeamsSql);

//		//	await ExecuteNonQueryAsync(insertTeamsSql);

//		//	logger.LogInformation($"Successfully added team entries to '{tableName}' table.");
//		//}

//		//public async Task UpdateRosterMappingsAsync(List<Roster> rosters)
//		//{
//		//	string playerTeamMapTableName = EntityMetadata.TableName(typeof(PlayerTeamMapSql));
//		//	var logger = GetLogger<TeamDbContext>();

//		//	logger.LogDebug($"Adding player-team mapping entries to '{playerTeamMapTableName}' table (will first remove all existing entries).");
//		//	logger.LogTrace($"Updating rosters for: {string.Join(", ", rosters.Select(r => r.TeamAbbreviation))}");

//		//	string playerTableName = EntityMetadata.TableName(typeof(PlayerSql));

//		//	List<PlayerSql> players = await SelectAsync<PlayerSql>($"SELECT id, nfl_id FROM {playerTableName};");
//		//	Dictionary<string, Guid> nflIdMap = players.ToDictionary(p => p.NflId, p => p.Id);

//		//	var sqlEntries = new List<PlayerTeamMapSql>();
//		//	foreach(Roster roster in rosters)
//		//	{
//		//		foreach(RosterPlayer player in roster.Players)
//		//		{
//		//			if (!nflIdMap.TryGetValue(player.NflId, out Guid id))
//		//			{
//		//				logger.LogWarning($"Player with NFL id '{player.NflId}' was found on the team '{roster.TeamAbbreviation}' page "
//		//					+ "but the player profile was unable to be fetched and resolved. Try updating rosters again later.");
//		//				continue;
//		//			}

//		//			sqlEntries.Add(PlayerTeamMapSql.ToSqlEntity(id, roster.TeamId));
//		//		}
//		//	}

//		//	string truncateCommand = $"TRUNCATE {playerTeamMapTableName};";
//		//	string insertCommands = SqlCommandBuilder.Rows.InsertMany(sqlEntries);

//		//	//await ExecuteTransactionWrappedAsync(new List<string>
//		//	//{
//		//	//	truncateCommand, insertCommands
//		//	//});

//		//	logger.LogInformation($"Successfully added player-team mapping entries to '{playerTeamMapTableName}' table.");
//		//}

//		//public async Task AddGameStatsAsync(List<TeamWeekStats> stats)
//		//{
//		//	string tableName = EntityMetadata.TableName(typeof(TeamGameStatsSql));
//		//	var logger = GetLogger<TeamDbContext>();

//		//	logger.LogDebug($"Adding team game stats to '{tableName}' table.");
//		//	logger.LogTrace($"Adding team game stats for: {string.Join(", ", stats.Select(s => s.Week))}");

//		//	List<TeamGameStatsSql> sqlEntries = stats.Select(TeamGameStatsSql.FromCoreEntity).ToList();

//		//	var sqlCommand = SqlCommandBuilder.Rows.InsertMany(sqlEntries);

//		//	await ExecuteNonQueryAsync(sqlCommand);
//		//	logger.LogInformation($"Successfully added team game stats to '{tableName}' table.");
//		//}

//		//public async Task RemoveAllGameStatsAsync()
//		//{
//		//	var logger = GetLogger<TeamDbContext>();
//		//	logger.LogInformation("Removing all team game stats rows from database.");

//		//	await ExecuteNonQueryAsync(SqlCommandBuilder.Rows.DeleteAll(typeof(TeamGameStatsSql)));

//		//	logger.LogInformation("Successfully removed all team game stats rows from database.");
//		//}

//		//public async Task RemoveGameStatsForWeekAsync(WeekInfo week)
//		//{
//		//	var logger = GetLogger<TeamDbContext>();
//		//	logger.LogInformation($"Removing team game stats rows for {week} from database.");

//		//	string tableName = EntityMetadata.TableName(typeof(TeamGameStatsSql));

//		//	string sqlCommand = $"DELETE FROM {tableName} WHERE season = {week.Season} AND week = {week.Week};";

//		//	await ExecuteNonQueryAsync(sqlCommand);

//		//	logger.LogInformation($"Successfully removed team game stats rows for {week} from database.");
//		//}

//		//public async Task AddGameMatchupsAsync(List<WeekMatchup> gameMatchups)
//		//{
//		//	string tableName = EntityMetadata.TableName(typeof(WeekGameMatchupSql));

//		//	var logger = GetLogger<TeamDbContext>();
//		//	logger.LogInformation($"Adding {gameMatchups.Count} week game matchup rows into '{tableName}' table.");

//		//	List<WeekGameMatchupSql> sqlEntries = gameMatchups.Select(WeekGameMatchupSql.FromCoreEntity).ToList();

//		//	var sqlCommand = SqlCommandBuilder.Rows.InsertMany(sqlEntries);

//		//	await ExecuteNonQueryAsync(sqlCommand);

//		//	logger.LogInformation($"Successfully added week game matchup rows to '{tableName}' table.");
//		//}
//	}
//}
