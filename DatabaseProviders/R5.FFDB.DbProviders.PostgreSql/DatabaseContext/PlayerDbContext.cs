using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Core.Models;
using R5.FFDB.Database;
using R5.FFDB.Database.DbContext;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		public async Task AddAsync(List<PlayerProfile> players, List<Roster> rosters)
		{
			var logger = GetLogger<PlayerDbContext>();
			string tableName = EntityInfoMap.TableName(typeof(PlayerSql));

			logger.LogInformation($"Adding {players.Count} players to the '{tableName}' table.");

			List<PlayerSql> playerSqls = ResolveSqlEntities(players, rosters);

			string sqlCommand = SqlCommandBuilder.Rows.InsertMany(playerSqls);

			logger.LogTrace($"Inserting players using SQL command:" + Environment.NewLine + sqlCommand);
			await ExecuteNonQueryAsync(sqlCommand);

			logger.LogInformation($"Successfully added players to the '{tableName}' table.");
		}

		public async Task UpdateAsync(List<PlayerProfile> players, List<Roster> rosters)
		{
			var logger = GetLogger<PlayerDbContext>();
			string tableName = EntityInfoMap.TableName(typeof(PlayerSql));

			logger.LogInformation($"Updating {players.Count} players in the '{tableName}' table.");

			List<PlayerSql> playerSqls = ResolveSqlEntities(players, rosters);

			foreach(PlayerSql player in playerSqls)
			{
				string sqlCommand = SqlCommandBuilder.Rows.Update(player);

				logger.LogTrace($"Updating player '{player.Id}' using SQL command:" + Environment.NewLine + sqlCommand);

				await ExecuteNonQueryAsync(sqlCommand);
			}

			logger.LogInformation($"Successfully updated players in the '{tableName}' table.");
		}

		private List<PlayerSql> ResolveSqlEntities(List<PlayerProfile> players, List<Roster> rosters)
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

		public async Task<List<PlayerProfile>> GetAllAsync()
		{
			var playerSqls = await SelectAsEntitiesAsync<PlayerSql>();
			return playerSqls.Select(PlayerSql.ToCoreEntity).ToList();
		}
	}
}
