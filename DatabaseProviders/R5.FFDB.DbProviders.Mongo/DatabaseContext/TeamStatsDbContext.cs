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
	public class TeamStatsDbContext : DbContextBase, ITeamStatsDbContext
	{
		public TeamStatsDbContext(
			Func<IMongoDatabase> getDatabase,
			ILoggerFactory loggerFactory)
			: base(getDatabase, loggerFactory)
		{
		}

		public async Task<List<TeamWeekStats>> GetAsync(WeekInfo week)
		{
			var logger = GetLogger<TeamStatsDbContext>();
			var collectionName = CollectionResolver.GetName<WeekStatsTeamDocument>();

			var builder = Builders<WeekStatsTeamDocument>.Filter;
			var filter = builder.Eq(s => s.Season, week.Season)
				& builder.Eq(s => s.Week, week.Week);

			List<WeekStatsTeamDocument> documents = await GetMongoDbContext().FindAsync(filter);

			logger.LogTrace($"Retrieved team week stats for week '{week}' from '{collectionName}' collection.");

			return documents.Select(WeekStatsTeamDocument.ToCoreEntity).ToList();
		}

		public async Task AddAsync(List<TeamWeekStats> stats)
		{
			if (stats == null)
			{
				throw new ArgumentNullException(nameof(stats), "Stats must be provided.");
			}

			var logger = GetLogger<TeamStatsDbContext>();
			var collectionName = CollectionResolver.GetName<WeekStatsTeamDocument>();

			logger.LogTrace($"Adding {stats.Count} team week stats to '{collectionName}' collection..");

			List<WeekStatsTeamDocument> documents = stats.Select(WeekStatsTeamDocument.FromCoreEntity).ToList();

			await GetMongoDbContext().InsertManyAsync(documents);

			logger.LogTrace($"Added team week stats to '{collectionName}' collection.");
		}
	}
}
