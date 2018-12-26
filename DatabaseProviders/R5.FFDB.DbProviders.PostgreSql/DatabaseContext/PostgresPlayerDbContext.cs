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
	public class PostgresPlayerDbContext : PostgresDbContextBase, IPlayerDatabaseContext
	{
		public PostgresPlayerDbContext(
			Func<NpgsqlConnection> getConnection,
			ILoggerFactory loggerFactory)
			: base(getConnection, loggerFactory)
		{
		}

		public async Task AddAsync(List<PlayerProfile> players, List<Roster> rosters, 
			int insertBatchCount = 500)
		{
			var logger = GetLogger<PostgresPlayerDbContext>();
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

			List<List<PlayerSql>> batches = Batch(playerSqls, insertBatchCount);

			logger.LogInformation($"Adding {playerSqls.Count} player entries to '{tableName} table in batches of {insertBatchCount} "
				+ $"(total insert commands: {batches.Count})");

			for (int i = 0; i < batches.Count; i++)
			{
				var sqlCommand = SqlCommandBuilder.Rows.InsertMany(batches[i]);

				logger.LogTrace($"Inserting batch {i + 1} using SQL command:" + Environment.NewLine + sqlCommand);

				await ExecuteNonQueryAsync(sqlCommand);
			}

			logger.LogInformation($"Successfully added players to the '{tableName}' table.");
		}

		// todo: move to shared/common
		private static List<List<T>> Batch<T>(List<T> items, int batchSize)
		{
			var result = new List<List<T>>();

			for (int i = 0; i < items.Count; i += batchSize)
			{
				result.Add(items.GetRange(i, Math.Min(batchSize, items.Count - i)));
			}

			return result;
		}

		public Task<List<PlayerProfile>> GetExistingAsync()
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(List<PlayerProfile> players, bool overrideExisting)
		{
			throw new NotImplementedException();
		}
	}
}
