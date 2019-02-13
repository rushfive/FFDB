using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using R5.FFDB.Core;
using R5.FFDB.Core.Database.DbContext;
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
	public class TeamDbContext : DbContextBase//, ITeamDatabaseContext
	{
		public TeamDbContext(
			Func<IMongoDatabase> getDatabase,
			ILoggerFactory loggerFactory)
			: base(getDatabase, loggerFactory)
		{
		}

		public async Task AddTeamsAsync()
		{
			ILogger<TeamDbContext> logger = GetLogger<TeamDbContext>();
			var collectionName = CollectionNames.GetForType<TeamDocument>();

			logger.LogDebug($"Adding NFL team documents to '{collectionName}' collection.");

			List<TeamDocument> teamSqls = TeamDataStore
				.GetAll()
				.Select(TeamDocument.FromCoreEntity)
				.ToList();

			MongoDbContext context = GetMongoDbContext();
			await context.InsertManyAsync(teamSqls);

			logger.LogInformation($"Successfully added team documents to '{collectionName}' collection.");
		}

		public async Task UpdateRosterMappingsAsync(List<Roster> rosters)
		{
			ILogger<TeamDbContext> logger = GetLogger<TeamDbContext>();
			var collectionName = CollectionNames.GetForType<PlayerDocument>();

			logger.LogDebug($"Updating player's team associations in '{collectionName}' collection.");
			logger.LogTrace($"Updating rosters for: {string.Join(", ", rosters.Select(r => r.TeamAbbreviation))}");

			MongoDbContext mongoDbContext = GetMongoDbContext();

			// first set all team ids to null
			var clearTeamUpdate = Builders<PlayerDocument>.Update.Set(p => p.TeamId, null);
			await mongoDbContext.UpdateAsync(clearTeamUpdate);

			var findOptions = new FindOptions<PlayerDocument>
			{
				Projection = Builders<PlayerDocument>.Projection
					.Include(p => p.Id)
					.Include(p => p.NflId)
			};

			List<PlayerDocument> playerDocuments = await mongoDbContext.FindAsync(findOptions: findOptions);
			Dictionary<string, Guid> nflIdMap = playerDocuments.ToDictionary(p => p.NflId, p => p.Id);

			foreach (Roster roster in rosters)
			{
				HashSet<Guid> playersHash = roster.Players
					.Where(p => nflIdMap.ContainsKey(p.NflId))
					.Select(p => nflIdMap[p.NflId])
					.ToHashSet();

				var update = Builders<PlayerDocument>.Update.Set(p => p.TeamId, roster.TeamId);
				var filter = Builders<PlayerDocument>.Filter.In(p => p.Id, playersHash);

				UpdateResult result = await mongoDbContext.UpdateAsync(update, filter);
				if (result.MatchedCount == 0)
				{
					throw new InvalidOperationException($"Updating roster ({nameof(PlayerDocument)}'s {nameof(PlayerDocument.TeamId)}) failed for team '{roster.TeamAbbreviation}'.");
				}
			}

			logger.LogInformation($"Successfully updated player's team associations in '{collectionName}' collection.");
		}

		public async Task AddGameStatsAsync(List<TeamWeekStats> stats)
		{
			ILogger<TeamDbContext> logger = GetLogger<TeamDbContext>();
			var collectionName = CollectionNames.GetForType<TeamGameStatsDocument>();

			logger.LogDebug($"Adding team game stats to '{collectionName}' collection.");
			logger.LogTrace($"Adding team game stats for: {string.Join(", ", stats.Select(s => s.Week))}");

			List<TeamGameStatsDocument> documents = stats.Select(TeamGameStatsDocument.FromCoreEntity).ToList();

			await GetMongoDbContext().InsertManyAsync(documents);

			logger.LogInformation($"Successfully added team game stats to '{collectionName}' collection.");
		}

		public async Task RemoveAllGameStatsAsync()
		{
			ILogger<TeamDbContext> logger = GetLogger<TeamDbContext>();
			var collectionName = CollectionNames.GetForType<TeamGameStatsDocument>();

			logger.LogDebug($"Removing all team game stats documents from '{collectionName}' collection.");

			await GetMongoDbContext().DeleteManyAsync<TeamGameStatsDocument>();

			logger.LogInformation($"Successfully removed all team game stats documents from '{collectionName}' collection.");
		}

		public async Task RemoveGameStatsForWeekAsync(WeekInfo week)
		{
			ILogger<TeamDbContext> logger = GetLogger<TeamDbContext>();
			var collectionName = CollectionNames.GetForType<TeamGameStatsDocument>();

			logger.LogDebug($"Removing team game stats documents for {week} from '{collectionName}' collection.");

			var filterBuilder = Builders<TeamGameStatsDocument>.Filter;
			FilterDefinition<TeamGameStatsDocument> filter = 
				filterBuilder.Eq(s => s.Season, week.Season)
				& filterBuilder.Eq(s => s.Week, week.Week);

			await GetMongoDbContext().DeleteManyAsync(filter);

			logger.LogInformation($"Successfully removed team game stats documents for {week} from '{collectionName}' collection.");
		}

		public async Task AddGameMatchupsAsync(List<WeekMatchup> gameMatchups)
		{
			ILogger<TeamDbContext> logger = GetLogger<TeamDbContext>();
			var collectionName = CollectionNames.GetForType<WeekGameMatchupDocument>();

			logger.LogDebug($"Adding week game matchup documents to '{collectionName}' collection.");

			List<WeekGameMatchupDocument> documents = gameMatchups.Select(WeekGameMatchupDocument.FromCoreEntity).ToList();

			await GetMongoDbContext().InsertManyAsync(documents);

			logger.LogInformation($"Successfully added week game matchup documents to '{collectionName}' collection.");
		}
	}
}
