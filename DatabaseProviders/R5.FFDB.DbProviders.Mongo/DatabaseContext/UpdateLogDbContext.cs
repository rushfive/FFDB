using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using R5.FFDB.Components;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.Mongo.Collections;
using R5.FFDB.DbProviders.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo.DatabaseContext
{
	public class UpdateLogDbContext : DbContextBase, IUpdateLogDbContext
	{
		public UpdateLogDbContext(
			Func<IMongoDatabase> getDatabase,
			IAppLogger logger)
			: base(getDatabase, logger)
		{
		}

		public async Task<List<WeekInfo>> GetAsync()
		{
			var collectionName = CollectionResolver.GetName<UpdateLogDocument>();

			var logs = await GetMongoDbContext().FindAsync<UpdateLogDocument>();

			Logger.LogDebug($"Retrieved updated weeks from '{collectionName}' collection.");

			return logs.Select(l => new WeekInfo(l.Season, l.Week)).ToList();
		}

		public async Task AddAsync(WeekInfo week)
		{
			var collectionName = CollectionResolver.GetName<UpdateLogDocument>();

			var log = new UpdateLogDocument
			{
				Season = week.Season,
				Week = week.Week,
				UpdateTime = DateTime.UtcNow
			};

			await GetMongoDbContext().InsertOneAsync(log);

			Logger.LogDebug($"Successfully added update log for {week} to '{collectionName}' collection.");
		}

		public async Task<bool> HasUpdatedWeekAsync(WeekInfo week)
		{
			var collectionName = CollectionResolver.GetName<UpdateLogDocument>();

			var builder = Builders<UpdateLogDocument>.Filter;
			var filter = builder.Eq(l => l.Season, week.Season)
				& builder.Eq(l => l.Week, week.Week);

			UpdateLogDocument log = await GetMongoDbContext().FindOneAsync(filter);

			return log != null;
		}
	}
}
