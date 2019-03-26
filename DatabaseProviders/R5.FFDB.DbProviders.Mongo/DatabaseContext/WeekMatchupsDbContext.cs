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
	public class WeekMatchupsDbContext : DbContextBase, IWeekMatchupsDbContext
	{
		public WeekMatchupsDbContext(
			Func<IMongoDatabase> getDatabase,
			IAppLogger logger)
			: base(getDatabase, logger)
		{
		}

		public async Task<List<WeekMatchup>> GetAsync(WeekInfo week)
		{
			var collectionName = CollectionResolver.GetName<WeekMatchupDocument>();

			var builder = Builders<WeekMatchupDocument>.Filter;
			var filter = builder.Eq(s => s.Season, week.Season)
				& builder.Eq(s => s.Week, week.Week);

			List<WeekMatchupDocument> documents = await GetMongoDbContext().FindAsync(filter);

			Logger.LogDebug($"Retrieved week matchups for week '{week}' from '{collectionName}' collection.");

			return documents.Select(WeekMatchupDocument.ToCoreEntity).ToList();
		}

		public async Task AddAsync(List<WeekMatchup> matchups)
		{
			if (matchups == null)
			{
				throw new ArgumentNullException(nameof(matchups), "Week matchups must be provided.");
			}
			
			var collectionName = CollectionResolver.GetName<WeekMatchupDocument>();

			Logger.LogDebug($"Adding {matchups.Count} week matchups to '{collectionName}' collection..");

			List<WeekMatchupDocument> documents = matchups.Select(WeekMatchupDocument.FromCoreEntity).ToList();

			await GetMongoDbContext().InsertManyAsync(documents);

			Logger.LogDebug($"Added week matchups to '{collectionName}' collection.");
		}
	}
}
