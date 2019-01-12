using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using R5.FFDB.Components.CoreData;
using R5.FFDB.Core.Models;
using R5.FFDB.Database.DbContext;
using R5.FFDB.DbProviders.Mongo.Collections;
using R5.FFDB.DbProviders.Mongo.Documents;
using R5.FFDB.DbProviders.Mongo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo.DatabaseContext
{
	public class WeekStatsDbContext : DbContextBase, IWeekStatsDatabaseContext
	{
		public WeekStatsDbContext(
			Func<IMongoDatabase> getDatabase,
			ILoggerFactory loggerFactory)
			: base(getDatabase, loggerFactory)
		{
		}

		public Task AddWeekAsync(WeekStats stats)
		{
			return AddWeeksAsync(new List<WeekStats> { stats });
		}

		public async Task AddWeeksAsync(List<WeekStats> stats)
		{
			var logger = GetLogger<WeekStatsDbContext>();
			var collectionName = CollectionNames.GetForType<WeekStatsPlayerDocument>();

			MongoDbContext mongoDbContext = GetMongoDbContext();

			var playerFindOptions = new FindOptions<PlayerDocument>
			{
				Projection = Builders<PlayerDocument>.Projection
					.Include(p => p.Id)
					.Include(p => p.NflId)
			};

			List<PlayerDocument> playerDocuments = await mongoDbContext.FindAsync(findOptions: playerFindOptions);

			var nflPlayerIdMap = playerDocuments.ToDictionary(p => p.NflId, p => p.Id);
			var teamNflIdMap = TeamDataStore.GetAll().ToDictionary(t => t.NflId, t => t.Id);
			
			List<WeekStatsDocumentAdd> statsAdd = GetStatsAdd(stats, nflPlayerIdMap, teamNflIdMap, logger);

			logger.LogInformation($"Adding week stats for {statsAdd.Count} week(s).");
			logger.LogTrace($"Adding week stats for: {string.Join(", ", stats.Select(s => s.Week))}");

			foreach(WeekStatsDocumentAdd add in statsAdd)
			{
				logger.LogDebug($"Beginning stats add for week {add.Week}.");

				if (add.PlayerStats.Any())
				{
					await mongoDbContext.InsertManyAsync(add.PlayerStats);
				}
				if (add.DstStats.Any())
				{
					await mongoDbContext.InsertManyAsync(add.DstStats);
				}

				logger.LogDebug($"Successfully added stats for week {add.Week}.");
			}

			logger.LogInformation($"Successfully finished adding week stats for {statsAdd.Count} weeks.");
		}

		private static List<WeekStatsDocumentAdd> GetStatsAdd(
			List<WeekStats> stats,
			Dictionary<string, Guid> nflPlayerIdMap,
			Dictionary<string, int> teamNflIdMap,
			ILogger<WeekStatsDbContext> logger)
		{
			var result = new List<WeekStatsDocumentAdd>();

			foreach (WeekStats weekStats in stats)
			{
				var update = new WeekStatsDocumentAdd
				{
					Week = weekStats.Week
				};

				foreach (PlayerWeekStats playerStats in weekStats.Players)
				{
					if (teamNflIdMap.TryGetValue(playerStats.NflId, out int teamId))
					{
						var statValues = WeekStatsDstDocument.FilterStatValues(playerStats);
						if (statValues.Any())
						{
							var dstStats = WeekStatsDstDocument.FromCoreEntity(teamId, weekStats.Week, statValues);
							update.DstStats.Add(dstStats);
						}

						continue;
					}

					if (nflPlayerIdMap.TryGetValue(playerStats.NflId, out Guid playerId))
					{
						WeekStatsPlayerDocument statsDocument = WeekStatsPlayerDocument.FromCoreEntity(playerStats, playerId, weekStats.Week);
						update.PlayerStats.Add(statsDocument);

						continue;
					}

					logger.LogWarning($"Failed to map NFL id '{playerStats.NflId}' to either a Team id or Player id. "
						+ $"They have stats recorded for week {weekStats.Week.Week} ({weekStats.Week.Season}) but cannot be added to the database.");
				}

				result.Add(update);
			}

			return result;
		}

		public async Task RemoveAllAsync()
		{
			var logger = GetLogger<WeekStatsDbContext>();

			MongoDbContext mongoDbContext = GetMongoDbContext();

			logger.LogInformation("Removing all week stats documents from database.");

			await mongoDbContext.DeleteManyAsync<WeekStatsPlayerDocument>();
			await mongoDbContext.DeleteManyAsync<WeekStatsDstDocument>();

			logger.LogInformation("Successfully removed all week stats documents from database.");
		}

		public async Task RemoveForWeekAsync(WeekInfo week)
		{
			var logger = GetLogger<WeekStatsDbContext>();
			var collectionName = CollectionNames.GetForType<WeekStatsPlayerDocument>();

			MongoDbContext mongoDbContext = GetMongoDbContext();

			logger.LogInformation($"Removing week stats documents for {week} from database.");

			await mongoDbContext.DeleteManyAsync<WeekStatsPlayerDocument>(ws => ws.Season == week.Season && ws.Week == week.Week);
			await mongoDbContext.DeleteManyAsync<WeekStatsDstDocument>(ws => ws.Season == week.Season && ws.Week == week.Week);

			logger.LogInformation($"Successfully removed week stats documents for {week} from database.");
		}

		
	}
}
