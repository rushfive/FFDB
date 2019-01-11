using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using R5.FFDB.Core.Models;
using R5.FFDB.Database.DbContext;
using R5.FFDB.DbProviders.Mongo.Collections;
using R5.FFDB.DbProviders.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo.DatabaseContext
{
	public class PlayerDbContext : DbContextBase, IPlayerDatabaseContext
	{
		public PlayerDbContext(
			Func<IMongoDatabase> getDatabase,
			ILoggerFactory loggerFactory)
			: base(getDatabase, loggerFactory)
		{
		}

		public async Task AddAsync(List<PlayerProfile> players, List<Roster> rosters)
		{
			var logger = GetLogger<PlayerDbContext>();
			var collectionName = CollectionNames.GetForType<PlayerDocument>();

			logger.LogInformation($"Adding {players.Count} players to the '{collectionName}' collection.");

			List<PlayerDocument> documents = MapToDocuments(players, rosters);

			await GetMongoDbContext().InsertManyAsync(documents);

			logger.LogInformation($"Successfully added {players.Count} players to the '{collectionName}' collection.");
		}

		public async Task UpdateAsync(List<PlayerProfile> players, List<Roster> rosters)
		{
			var logger = GetLogger<PlayerDbContext>();
			var collectionName = CollectionNames.GetForType<PlayerDocument>();

			logger.LogInformation($"Updating {players.Count} players in the '{collectionName}' collection.");

			MongoDbContext mongoDbContext = GetMongoDbContext();
			List<PlayerDocument> documents = MapToDocuments(players, rosters);

			foreach(var player in documents)
			{
				await mongoDbContext.ReplaceOneAsync(p => p.Id == player.Id, player);
			}

			logger.LogInformation($"Successfully updated {players.Count} players in the '{collectionName}' collection.");
		}

		private List<PlayerDocument> MapToDocuments(List<PlayerProfile> players, List<Roster> rosters)
		{
			var result = new List<PlayerDocument>();

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

				var document = PlayerDocument.FromCoreEntity(player, number, position, status);
				result.Add(document);
			}

			return result;
		}

		public async Task<List<PlayerProfile>> GetAllAsync()
		{
			var logger = GetLogger<PlayerDbContext>();
			var collectionName = CollectionNames.GetForType<PlayerDocument>();

			List<PlayerDocument> documents = await GetMongoDbContext().FindAsync<PlayerDocument>();
			var result = documents.Select(PlayerDocument.ToCoreEntity).ToList();

			logger.LogInformation($"Successfully retrieved all players from '{collectionName}' collection.");

			return result;
		}

		
	}
}
