using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using R5.FFDB.Components;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.FFDB.DbProviders.Mongo.Collections;
using R5.FFDB.DbProviders.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo.DatabaseContext
{
	public class TeamDbContext : DbContextBase, ITeamDbContext
	{
		public TeamDbContext(
			Func<IMongoDatabase> getDatabase,
			IAppLogger logger)
			: base(getDatabase, logger)
		{
		}

		public async Task<List<int>> GetExistingTeamIdsAsync()
		{
			var findOptions = new FindOptions<TeamDocument, int>
			{
				Projection = Builders<TeamDocument>.Projection
					.Expression(t => t.Id)
			};

			return await GetMongoDbContext().FindAsync(findOptions: findOptions) ?? new List<int>();
		}

		public async Task AddAsync(List<Team> teams)
		{
			if (teams == null)
			{
				throw new ArgumentNullException(nameof(teams), "Teams must be provided.");
			}
			
			var collectionName = CollectionResolver.GetName<TeamDocument>();
			
			if (!teams.Any())
			{
				return;
			}

			var docs = teams.Select(TeamDocument.FromCoreEntity).ToList();

			await GetMongoDbContext().InsertManyAsync(docs);

			Logger.LogDebug($"Added {teams.Count} teams to '{collectionName}' collection.");
		}

		// first set all player's team ids to null, then update
		public async Task UpdateRosterMappingsAsync(List<Roster> rosters)
		{
			if (rosters == null)
			{
				throw new ArgumentNullException(nameof(rosters), "Rosters must be provided.");
			}
			
			var collectionName = CollectionResolver.GetName<PlayerDocument>();

			Logger.LogDebug($"Updating roster mappings..");

			MongoDbContext mongoDbContext = GetMongoDbContext();
			
			await ClearRosterMappingsAsync(mongoDbContext);
			
			Dictionary<string, Guid> nflIdMap = await GetNflIdMapAsync(mongoDbContext);

			foreach (Roster roster in rosters)
			{
				await UpdateForRosterAsync(roster, nflIdMap, mongoDbContext);
			}

			Logger.LogDebug($"Updated roster mappings for players in '{collectionName}' collection.");
		}

		private Task ClearRosterMappingsAsync(MongoDbContext mongoDbContext)
		{
			var clearUpdate = Builders<PlayerDocument>.Update.Set(p => p.TeamId, null);

			return mongoDbContext.UpdateAsync(clearUpdate);
		}

		private async Task<Dictionary<string, Guid>> GetNflIdMapAsync(MongoDbContext mongoDbContext)
		{
			var findOptions = new FindOptions<PlayerDocument>
			{
				Projection = Builders<PlayerDocument>.Projection
					.Include(p => p.Id)
					.Include(p => p.NflId)
			};

			List<PlayerDocument> players = await mongoDbContext.FindAsync(findOptions: findOptions);

			return players.ToDictionary(p => p.NflId, p => p.Id, StringComparer.OrdinalIgnoreCase);
		}

		private async Task UpdateForRosterAsync(Roster roster, 
			Dictionary<string, Guid> nflIdMap, MongoDbContext mongoDbContext)
		{
			var playerIds = roster.Players
				.Where(p => nflIdMap.ContainsKey(p.NflId))
				.Select(p => nflIdMap[p.NflId]);

			var update = Builders<PlayerDocument>.Update.Set(p => p.TeamId, roster.TeamId);
			var filter = Builders<PlayerDocument>.Filter.In(p => p.Id, playerIds);

			UpdateResult result = await mongoDbContext.UpdateAsync(update, filter);
			if (result.MatchedCount == 0)
			{
				throw new InvalidOperationException($"Updating roster mappings failed for team '{roster.TeamAbbreviation}'.");
			}
		}
	}
}
