using Microsoft.Extensions.Logging;
using R5.FFDB.Components;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Entities;
using R5.Internals.PostgresMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public class PlayerDbContext : DbContextBase, IPlayerDbContext
	{
		public PlayerDbContext(DbConnection dbConnection, IAppLogger logger)
			: base(dbConnection, logger)
		{
		}

		public async Task<List<Player>> GetAllAsync()
		{
			Logger.LogDebug($"Getting all players from '{MetadataResolver.TableName<PlayerSql>()}' table.");

			List<PlayerSql> sqlEntries = await DbConnection.Select<PlayerSql>().ExecuteAsync();

			return sqlEntries.Select(PlayerSql.ToCoreEntity).ToList();
		}

		public Task AddAsync(PlayerAdd player)
		{
			if (player == null)
			{
				throw new ArgumentNullException(nameof(player), "Player add must be provided.");
			}

			Logger.LogDebug($"Adding player '{player}' to '{MetadataResolver.TableName<PlayerSql>()}' table.");

			PlayerSql sqlEntry = PlayerSql.FromCoreAddEntity(player);

			return DbConnection.Insert(sqlEntry).ExecuteAsync();
		}

		public Task UpdateAsync(Guid id, PlayerUpdate update)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentException("Player id must be provided.", nameof(id));
			}
			if (update == null)
			{
				throw new ArgumentNullException(nameof(update), "Update must be provided.");
			}

			Logger.LogDebug($"Updating player '{id}' in '{MetadataResolver.TableName<PlayerSql>()}' table.");

			return DbConnection.Update<PlayerSql>()
				.Where(p => p.Id == id)
				.Set(p => p.Number, update.Number)
				.Set(p => p.Position, update.Position)
				.Set(p => p.Status, update.Status)
				.ExecuteAsync();
		}
	}
}