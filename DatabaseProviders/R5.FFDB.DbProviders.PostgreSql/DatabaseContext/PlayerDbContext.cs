using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Core.Models;
using R5.FFDB.Database;
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

		public async Task UpdateAsync(List<PlayerProfile> players, List<Roster> rosters)
		{
			var logger = GetLogger<PlayerDbContext>();
			string tableName = EntityInfoMap.TableName(typeof(PlayerSql));

			logger.LogInformation($"Adding {players.Count} players to the '{tableName}' table.");

			// need latest player team, position and number from roster info
			logger.LogTrace("Building roster player map to resolve necessary information for player adds.");

			Dictionary<string, RosterPlayer> rosterPlayerMap = rosters
				.SelectMany(r => r.Players)
				.ToDictionary(p => p.NflId, p => p);

			var playerSqls = new List<PlayerSql>();
			foreach (var player in players)
			{
				int? number = null;
				Position? position = null;
				if (rosterPlayerMap.TryGetValue(player.NflId, out RosterPlayer rosterPlayer))
				{
					number = rosterPlayer.Number;
					position = rosterPlayer.Position;
				}

				PlayerSql entitySql = PlayerSql.FromCoreEntity(player, number, position);
				playerSqls.Add(entitySql);

				logger.LogTrace($"Successfully mapped player information for {player.FirstName} {player.LastName} ({player.NflId}). "
					+ $"Will use '{tableName}' table id '{entitySql.Id}'.");
			}

			string sqlCommand = SqlCommandBuilder.Rows.InsertMany(playerSqls);

			logger.LogTrace($"Inserting players using SQL command:" + Environment.NewLine + sqlCommand);
			await ExecuteNonQueryAsync(sqlCommand);

			logger.LogInformation($"Successfully added players to the '{tableName}' table.");
		}

		public async Task<List<PlayerProfile>> GetAllAsync()
		{
			var playerSqls = await SelectAsEntitiesAsync<PlayerSql>();
			return playerSqls.Select(PlayerSql.ToCoreEntity).ToList();
		}
	}
}
