using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.Mongo.Collections;
using R5.FFDB.DbProviders.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo.DatabaseContext
{
	public class LogDbContext : DbContextBase, ILogDatabaseContext
	{
		public LogDbContext(
			Func<IMongoDatabase> getDatabase,
			ILoggerFactory loggerFactory)
			: base(getDatabase, loggerFactory)
		{
		}

		public async Task AddUpdateForWeekAsync(WeekInfo week)
		{
			var logger = GetLogger<LogDbContext>();
			var collectionName = CollectionNames.GetForType<UpdateLogDocument>();

			var log = new UpdateLogDocument
			{
				Season = week.Season,
				Week = week.Week,
				UpdateTime = DateTime.UtcNow
			};
			
			await GetMongoDbContext().InsertOneAsync(log);

			logger.LogInformation($"Successfully added update log for {week} to '{collectionName}' collection.");
		}

		public async Task<List<WeekInfo>> GetUpdatedWeeksAsync()
		{
			var logger = GetLogger<LogDbContext>();
			var collectionName = CollectionNames.GetForType<UpdateLogDocument>();

			logger.LogInformation($"Getting updated weeks from '{collectionName}' collection.");

			var logs = await GetMongoDbContext().FindAsync<UpdateLogDocument>();

			return logs.Select(l => new WeekInfo(l.Season, l.Week)).ToList();
		}

		public async Task<bool> HasUpdatedWeekAsync(WeekInfo week)
		{
			var collectionName = CollectionNames.GetForType<UpdateLogDocument>();

			var builder = Builders<UpdateLogDocument>.Filter;
			var filter = builder.Eq(l => l.Season, week.Season)
				& builder.Eq(l => l.Week, week.Week);

			UpdateLogDocument log = await GetMongoDbContext().FindOneAsync(filter);

			return log != null;
		}

		public async Task RemoveAllAsync()
		{
			var logger = GetLogger<LogDbContext>();
			var collectionName = CollectionNames.GetForType<UpdateLogDocument>();

			await GetMongoDbContext().DeleteManyAsync<UpdateLogDocument>();

			logger.LogInformation($"Deleted all log documents from '{collectionName}' collection.");
		}

		public async Task RemoveForWeekAsync(WeekInfo week)
		{
			var logger = GetLogger<LogDbContext>();
			var collectionName = CollectionNames.GetForType<UpdateLogDocument>();

			logger.LogDebug($"Deleting log document for {week} from '{collectionName}' collection.");

			DeleteResult result = await GetMongoDbContext().DeleteOneAsync<UpdateLogDocument>(l => l.Season == week.Season && l.Week == week.Week);
			if (result.DeletedCount != 1)
			{
				throw new InvalidOperationException($"Failed to delete log document for {week} from '{collectionName}' collection.");
			}

			logger.LogInformation($"Successfully deleted log document for {week} from '{collectionName}' collection.");
		}
	}
}
