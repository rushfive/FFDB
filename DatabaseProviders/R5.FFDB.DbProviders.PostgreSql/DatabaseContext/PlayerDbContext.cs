using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Core.Database.DbContext;
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
	public class PlayerDbContext : DbContextBase, IPlayerDatabaseContext
	{
		public PlayerDbContext(
			Func<NpgsqlConnection> getConnection,
			ILoggerFactory loggerFactory)
			: base(getConnection, loggerFactory)
		{
		}

		// NEW for pipeline

		public Task AddAsync(Player player)
		{
			throw new NotImplementedException();
		}

		// OLD BELOW

		public async Task AddAsync(List<Player> players, List<Roster> rosters)
		{
			var logger = GetLogger<PlayerDbContext>();
			string tableName = EntityInfoMap.TableName(typeof(PlayerSql));

			logger.LogInformation($"Adding {players.Count} players to the '{tableName}' table.");

			List<PlayerSql> playerSqls = MapToSqlEntities(players, rosters);

			string sqlCommand = SqlCommandBuilder.Rows.InsertMany(playerSqls);

			logger.LogTrace($"Inserting players using SQL command:" + Environment.NewLine + sqlCommand);
			await ExecuteNonQueryAsync(sqlCommand);

			logger.LogInformation($"Successfully added players to the '{tableName}' table.");
		}

		public async Task UpdateAsync(List<Player> players, List<Roster> rosters)
		{
			var logger = GetLogger<PlayerDbContext>();
			string tableName = EntityInfoMap.TableName(typeof(PlayerSql));

			logger.LogInformation($"Updating {players.Count} players in the '{tableName}' table.");

			List<PlayerSql> playerSqls = MapToSqlEntities(players, rosters);

			foreach (PlayerSql player in playerSqls)
			{
				string sqlCommand = SqlCommandBuilder.Rows.Update(player);

				logger.LogTrace($"Updating player '{player.Id}' using SQL command:" + Environment.NewLine + sqlCommand);

				await ExecuteNonQueryAsync(sqlCommand);
			}

			logger.LogInformation($"Successfully updated players in the '{tableName}' table.");
		}

		private List<PlayerSql> MapToSqlEntities(List<Player> players, List<Roster> rosters)
		{
			var result = new List<PlayerSql>();

			Dictionary<string, RosterPlayer> rosterPlayerMap = rosters
				.SelectMany(r => r.Players)
				.ToDictionary(p => p.NflId, p => p);

			foreach (var player in players)
			{
				int? number = null;
				Position? position = null;
				RosterStatus? status = null;
				if (rosterPlayerMap.TryGetValue(player.NflId, out RosterPlayer rosterPlayer))
				{
					number = rosterPlayer.Number;
					position = rosterPlayer.Position;
					status = rosterPlayer.Status;
				}

				PlayerSql entitySql = PlayerSql.FromCoreEntity(player, number, position, status);
				result.Add(entitySql);
			}

			return result;
		}

		public async Task<List<Player>> GetAllAsync()
		{
			var playerSqls = await SelectAsEntitiesAsync<PlayerSql>();
			return playerSqls.Select(PlayerSql.ToCoreEntity).ToList();
		}

		public async Task<List<Player>> GetByTeamForWeekAsync(int teamId, WeekInfo week)
		{
			var player = EntityInfoMap.TableName(typeof(PlayerSql));

			var tables = new List<string>
			{
				EntityInfoMap.TableName(typeof(WeekStatsRushSql)),
				EntityInfoMap.TableName(typeof(WeekStatsReturnSql)),
				EntityInfoMap.TableName(typeof(WeekStatsReceiveSql)),
				EntityInfoMap.TableName(typeof(WeekStatsPassSql)),
				EntityInfoMap.TableName(typeof(WeekStatsMiscSql)),
				EntityInfoMap.TableName(typeof(WeekStatsKickSql)),
				EntityInfoMap.TableName(typeof(WeekStatsIdpSql))
			};

			IEnumerable<string> idsByTable = tables
				.Select(t => $"SELECT player_id FROM {t} WHERE season = {week.Season} AND week = {week.Week} AND team_id = {teamId}");

			string sql = $"SELECT * FROM {player} WHERE id in ({string.Join(" UNION ALL ", idsByTable)});";

			var playerSqls = await SelectAsEntitiesAsync<PlayerSql>(sql);
			return playerSqls.Select(PlayerSql.ToCoreEntity).ToList();
		}

		public Task UpdateAsync(Player player)
		{
			throw new NotImplementedException();
		}
	}
}
