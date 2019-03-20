using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using R5.FFDB.Components;
using R5.FFDB.Core;
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
	public class PlayerStatsDbContext : DbContextBase, IPlayerStatsDbContext
	{
		public PlayerStatsDbContext(
			Func<IMongoDatabase> getDatabase,
			IAppLogger logger)
			: base(getDatabase, logger)
		{
		}

		public async Task<List<string>> GetPlayerNflIdsAsync(WeekInfo week)
		{
			var collectionName = CollectionResolver.GetName<WeekStatsPlayerDocument>();

			MongoDbContext mongoDbContext = GetMongoDbContext();

			List<Guid> ids = await GetPlayerIdsAsync(week, mongoDbContext);

			Dictionary<Guid, string> idNflMap = await GetIdNflMapAsync(mongoDbContext);

			return ids
				.Where(id => idNflMap.ContainsKey(id))
				.Select(id => idNflMap[id])
				.ToList();
		}

		private Task<List<Guid>> GetPlayerIdsAsync(WeekInfo week, MongoDbContext mongoDbContext)
		{
			var builder = Builders<WeekStatsPlayerDocument>.Filter;
			var filter = builder.Eq(s => s.Season, week.Season)
				& builder.Eq(s => s.Week, week.Week);

			var findOptions = new FindOptions<WeekStatsPlayerDocument, Guid>
			{
				Projection = Builders<WeekStatsPlayerDocument>.Projection
					.Expression(p => p.PlayerId)
			};

			return mongoDbContext.FindAsync(filter, findOptions);
		}

		private async Task<Dictionary<Guid, string>> GetIdNflMapAsync(MongoDbContext mongoDbContext)
		{
			var playerFindOptions = new FindOptions<PlayerDocument>
			{
				Projection = Builders<PlayerDocument>.Projection
					.Include(p => p.Id)
					.Include(p => p.NflId)
			};

			List<PlayerDocument> playerDocuments = await mongoDbContext.FindAsync(findOptions: playerFindOptions);

			return playerDocuments.ToDictionary(p => p.Id, p => p.NflId);
		}

		public async Task AddAsync(List<PlayerWeekStats> stats)
		{
			if (stats == null)
			{
				throw new ArgumentNullException(nameof(stats), "Stats must be provided.");
			}

			MongoDbContext mongoDbContext = GetMongoDbContext();
			
			Logger.LogDebug($"Adding {stats.Count} week stats..");
			
			var (playerStats, dstStats) = GroupStats(stats);
			
			await AddPlayerStatsAsync(playerStats, mongoDbContext);

			Logger.LogDebug("Added player week stats to '{0}' collection.",
				CollectionResolver.GetName<WeekStatsPlayerDocument>());

			await AddDstStatsAsync(dstStats, mongoDbContext);

			Logger.LogDebug("Added DST week stats to '{0}' collection.",
				CollectionResolver.GetName<WeekStatsDstDocument>());
		}

		private (List<PlayerWeekStats> player, List<PlayerWeekStats> dst) GroupStats(List<PlayerWeekStats> stats)
		{
			var playerStats = new List<PlayerWeekStats>();
			var dstStats = new List<PlayerWeekStats>();

			foreach (var s in stats)
			{
				if (Teams.IsTeam(s.NflId))
				{
					dstStats.Add(s);
				}
				else
				{
					playerStats.Add(s);
				}
			}

			return (playerStats, dstStats);
		}

		private async Task AddPlayerStatsAsync(List<PlayerWeekStats> stats, MongoDbContext mongoDbContext)
		{
			Dictionary<string, Guid> nflIdMap = await GetNflIdMapAsync(mongoDbContext);

			List<WeekStatsPlayerDocument> playerStats = stats
				.Where(s => nflIdMap.ContainsKey(s.NflId))
				.Select(s => WeekStatsPlayerDocument.FromCoreEntity(s, nflIdMap))
				.ToList();

			await mongoDbContext.InsertManyAsync(playerStats);
		}

		private async Task<Dictionary<string, Guid>> GetNflIdMapAsync(MongoDbContext mongoDbContext)
		{
			var playerFindOptions = new FindOptions<PlayerDocument>
			{
				Projection = Builders<PlayerDocument>.Projection
					.Include(p => p.Id)
					.Include(p => p.NflId)
			};

			List<PlayerDocument> playerDocuments = await mongoDbContext.FindAsync(findOptions: playerFindOptions);

			return playerDocuments.ToDictionary(p => p.NflId, p => p.Id, StringComparer.OrdinalIgnoreCase);
		}

		private async Task AddDstStatsAsync(List<PlayerWeekStats> stats, MongoDbContext mongoDbContext)
		{
			List<WeekStatsDstDocument> dstStats = stats
				.Select(WeekStatsDstDocument.FromCoreEntity)
				.ToList();

			await mongoDbContext.InsertManyAsync(dstStats);
		}
	}
}
