using Microsoft.Extensions.Logging;
using MongoDB.Driver;
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
			ILoggerFactory loggerFactory)
			: base(getDatabase, loggerFactory)
		{
		}

		public Task<List<Player>> GetAllAsync()
		{
			throw new NotImplementedException();
		}

		public Task AddAsync(PlayerAdd player)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(Guid id, PlayerUpdate update)
		{
			throw new NotImplementedException();
		}

		// NEW FOR PIPELINE

		//public async Task AddAsync(Player player)
		//{
		//	PlayerDocument document = PlayerDocument.FromCoreEntity(player);

		//	await GetMongoDbContext().InsertOneAsync(document);

		//	var collectionName = CollectionNames.GetForType<PlayerDocument>();
		//	GetLogger<PlayerDbContext>().LogTrace($"Saved player {player} to collection '{collectionName}'.");
		//}

		//public async Task UpdateAsync(Player player)
		//{
		//	PlayerDocument document = PlayerDocument.FromCoreEntity(player);

		//	await GetMongoDbContext().ReplaceOneAsync(p => p.Id == player.Id, document);

		//	var collectionName = CollectionNames.GetForType<PlayerDocument>();
		//	GetLogger<PlayerDbContext>().LogTrace($"Updated player {player} in collection '{collectionName}'.");
		//}

		//// OLD BELOW

		//public async Task AddAsync(List<Player> players, List<Roster> rosters)
		//{
		//	var logger = GetLogger<PlayerDbContext>();
		//	var collectionName = CollectionNames.GetForType<PlayerDocument>();

		//	logger.LogInformation($"Adding {players.Count} players to the '{collectionName}' collection.");

		//	List<PlayerDocument> documents = MapToDocuments(players, rosters);

		//	await GetMongoDbContext().InsertManyAsync(documents);

		//	logger.LogInformation($"Successfully added {players.Count} players to the '{collectionName}' collection.");
		//}

		//public async Task UpdateAsync(List<Player> players, List<Roster> rosters)
		//{
		//	var logger = GetLogger<PlayerDbContext>();
		//	var collectionName = CollectionNames.GetForType<PlayerDocument>();

		//	logger.LogInformation($"Updating {players.Count} players in the '{collectionName}' collection.");

		//	MongoDbContext mongoDbContext = GetMongoDbContext();
		//	List<PlayerDocument> documents = MapToDocuments(players, rosters);

		//	foreach(var player in documents)
		//	{
		//		await mongoDbContext.ReplaceOneAsync(p => p.Id == player.Id, player);
		//	}

		//	logger.LogInformation($"Successfully updated {players.Count} players in the '{collectionName}' collection.");
		//}

		//private List<PlayerDocument> MapToDocuments(List<Player> players, List<Roster> rosters)
		//{
		//	var result = new List<PlayerDocument>();

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

		//		//var document = PlayerDocument.FromCoreEntity(player, number, position, status);
		//		var document = PlayerDocument.FromCoreEntity(player);
		//		result.Add(document);
		//	}

		//	return result;
		//}

		//public async Task<List<Player>> GetAllAsync()
		//{
		//	var logger = GetLogger<PlayerDbContext>();
		//	var collectionName = CollectionNames.GetForType<PlayerDocument>();

		//	List<PlayerDocument> documents = await GetMongoDbContext().FindAsync<PlayerDocument>();
		//	var result = documents.Select(PlayerDocument.ToCoreEntity).ToList();

		//	logger.LogInformation($"Successfully retrieved all players from '{collectionName}' collection.");

		//	return result;
		//}

		//public async Task<List<Player>> GetByTeamForWeekAsync(int teamId, WeekInfo week)
		//{
		//	var builder = Builders<WeekStatsPlayerDocument>.Filter;

		//	FilterDefinition<WeekStatsPlayerDocument> filter = builder.And(
		//		builder.Eq(ws => ws.TeamId, teamId),
		//		builder.Eq(ws => ws.Season, week.Season),
		//		builder.Eq(ws => ws.Week, week.Week));

		//	var findOptions = new FindOptions<WeekStatsPlayerDocument, Guid>
		//	{
		//		Projection = Builders<WeekStatsPlayerDocument>.Projection
		//			.Expression(ws => ws.PlayerId)
		//	};

		//	MongoDbContext mongoDbContext = GetMongoDbContext();

		//	List<Guid> ids = await mongoDbContext.FindAsync(filter, findOptions);

		//	var playerFilter = Builders<PlayerDocument>.Filter.In(p => p.Id, ids);
		//	List<PlayerDocument> players = await mongoDbContext.FindAsync(playerFilter);

		//	return players.Select(PlayerDocument.ToCoreEntity).ToList();
		//}
	}
}
