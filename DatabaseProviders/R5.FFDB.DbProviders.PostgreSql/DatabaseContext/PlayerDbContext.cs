using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities.WeekStats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public class PlayerDbContext : DbContextBase, IPlayerDbContext
	{
		public PlayerDbContext(
			Func<NpgsqlConnection> getConnection,
			ILoggerFactory loggerFactory)
			: base(getConnection, loggerFactory)
		{
		}

		public async Task<List<Player>> GetAllAsync()
		{
			var players = await GetPostgresDbContext().SelectAsync<PlayerSql>();

			var logger = GetLogger<PlayerDbContext>();
			logger.LogTrace($"Retrieved all players from '{EntityMetadata.TableName<PlayerSql>()}' table.");

			return players.Select(PlayerSql.ToCoreEntity).ToList();
		}

		public async Task AddAsync(PlayerAdd player)
		{
			if (player == null)
			{
				throw new ArgumentNullException(nameof(player), "Player add model must be provided.");
			}
			
			PlayerSql playerSql = PlayerSql.FromCoreAddEntity(player);

			await GetPostgresDbContext().InsertAsync(playerSql);

			var logger = GetLogger<PlayerDbContext>();
			logger.LogTrace($"Added player '{player.NflId}' as '{playerSql.Id}' to '{EntityMetadata.TableName<PlayerSql>()}' table.");
		}

		

		public async Task UpdateAsync(Guid id, PlayerUpdate update)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentException("", nameof(id));
			}
			if (update == null)
			{
				throw new ArgumentNullException(nameof(update), "Player update model must be provided.");
			}

			var logger = GetLogger<PlayerDbContext>();
			//var collectionName = CollectionResolver.GetName<PlayerDocument>();

			logger.LogTrace($"Updating player '{id}'..");

			throw new NotImplementedException();
		}



		//// NEW for pipeline

		//public Task AddAsync(Player player)
		//{
		//	throw new NotImplementedException();
		//}

		//// OLD BELOW

		//public async Task AddAsync(List<Player> players, List<Roster> rosters)
		//{
		//	var logger = GetLogger<PlayerDbContext>();
		//	string tableName = EntityMetadata.TableName(typeof(PlayerSql));

		//	logger.LogInformation($"Adding {players.Count} players to the '{tableName}' table.");

		//	List<PlayerSql> playerSqls = MapToSqlEntities(players, rosters);

		//	string sqlCommand = SqlCommandBuilder.Rows.InsertMany(playerSqls);

		//	logger.LogTrace($"Inserting players using SQL command:" + Environment.NewLine + sqlCommand);
		//	await ExecuteNonQueryAsync(sqlCommand);

		//	logger.LogInformation($"Successfully added players to the '{tableName}' table.");
		//}

		//public async Task UpdateAsync(List<Player> players, List<Roster> rosters)
		//{
		//	var logger = GetLogger<PlayerDbContext>();
		//	string tableName = EntityMetadata.TableName(typeof(PlayerSql));

		//	logger.LogInformation($"Updating {players.Count} players in the '{tableName}' table.");

		//	List<PlayerSql> playerSqls = MapToSqlEntities(players, rosters);

		//	foreach (PlayerSql player in playerSqls)
		//	{
		//		string sqlCommand = SqlCommandBuilder.Rows.Update(player);

		//		logger.LogTrace($"Updating player '{player.Id}' using SQL command:" + Environment.NewLine + sqlCommand);

		//		await ExecuteNonQueryAsync(sqlCommand);
		//	}

		//	logger.LogInformation($"Successfully updated players in the '{tableName}' table.");
		//}

		//private List<PlayerSql> MapToSqlEntities(List<Player> players, List<Roster> rosters)
		//{
		//	var result = new List<PlayerSql>();

		//	Dictionary<string, RosterPlayer> rosterPlayerMap = rosters
		//		.SelectMany(r => r.Players)
		//		.ToDictionary(p => p.NflId, p => p);

		//	foreach (var player in players)
		//	{
		//		int? number = null;
		//		Position? position = null;
		//		RosterStatus? status = null;
		//		if (rosterPlayerMap.TryGetValue(player.NflId, out RosterPlayer rosterPlayer))
		//		{
		//			number = rosterPlayer.Number;
		//			position = rosterPlayer.Position;
		//			status = rosterPlayer.Status;
		//		}

		//		PlayerSql entitySql = PlayerSql.FromCoreEntity(player, number, position, status);
		//		result.Add(entitySql);
		//	}

		//	return result;
		//}

		//public async Task<List<Player>> GetAllAsync()
		//{
		//	var playerSqls = await SelectAsync<PlayerSql>();
		//	return playerSqls.Select(PlayerSql.ToCoreEntity).ToList();
		//}

		//public async Task<List<Player>> GetByTeamForWeekAsync(int teamId, WeekInfo week)
		//{
		//	var player = EntityMetadata.TableName(typeof(PlayerSql));

		//	var tables = new List<string>
		//	{
		//		EntityMetadata.TableName(typeof(WeekStatsRushSql)),
		//		EntityMetadata.TableName(typeof(WeekStatsReturnSql)),
		//		EntityMetadata.TableName(typeof(WeekStatsReceiveSql)),
		//		EntityMetadata.TableName(typeof(WeekStatsPassSql)),
		//		EntityMetadata.TableName(typeof(WeekStatsMiscSql)),
		//		EntityMetadata.TableName(typeof(WeekStatsKickSql)),
		//		EntityMetadata.TableName(typeof(WeekStatsIdpSql))
		//	};

		//	IEnumerable<string> idsByTable = tables
		//		.Select(t => $"SELECT player_id FROM {t} WHERE season = {week.Season} AND week = {week.Week} AND team_id = {teamId}");

		//	string sql = $"SELECT * FROM {player} WHERE id in ({string.Join(" UNION ALL ", idsByTable)});";

		//	var playerSqls = await SelectAsync<PlayerSql>(sql);
		//	return playerSqls.Select(PlayerSql.ToCoreEntity).ToList();
		//}

		//public Task UpdateAsync(Player player)
		//{
		//	throw new NotImplementedException();
		//}
	}
}
