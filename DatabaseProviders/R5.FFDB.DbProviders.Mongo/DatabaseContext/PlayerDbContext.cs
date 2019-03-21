using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using R5.FFDB.Components;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.Mongo.Collections;
using R5.FFDB.DbProviders.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo.DatabaseContext
{
	public class PlayerDbContext : DbContextBase, IPlayerDbContext
	{
		public PlayerDbContext(
			Func<IMongoDatabase> getDatabase,
			IAppLogger logger)
			: base(getDatabase, logger)
		{
		}

		public async Task<List<Player>> GetAllAsync()
		{
			var collectionName = CollectionResolver.GetName<PlayerDocument>();

			List<PlayerDocument> documents = await GetMongoDbContext().FindAsync<PlayerDocument>();

			Logger.LogDebug($"Retrieved all players from '{collectionName}' collection.");

			return documents.Select(PlayerDocument.ToCoreEntity).ToList();
		}

		public async Task AddAsync(PlayerAdd player)
		{
			if (player == null)
			{
				throw new ArgumentNullException(nameof(player), "Player add model must be provided.");
			}
			
			var collectionName = CollectionResolver.GetName<PlayerDocument>();

			PlayerDocument document = PlayerDocument.FromCoreAddEntity(player);

			await GetMongoDbContext().InsertOneAsync(document);

			Logger.LogDebug($"Added player '{player.NflId}' as '{document.Id}' to '{collectionName}' collection.");
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
			
			var collectionName = CollectionResolver.GetName<PlayerDocument>();

			Logger.LogDebug($"Updating player '{id}'..");

			var updateDefinition = Builders<PlayerDocument>.Update
				.Set(p => p.Number, update.Number)
				.Set(p => p.Position, update.Position)
				.Set(p => p.Status, update.Status);

			UpdateResult result = await GetMongoDbContext().UpdateOneAsync(p => p.Id == id, updateDefinition);
			if (result.MatchedCount != 1)
			{
				throw new InvalidOperationException($"Failed to update player '{id}' because it doesn't exist.");
			}
		}
	}
}
