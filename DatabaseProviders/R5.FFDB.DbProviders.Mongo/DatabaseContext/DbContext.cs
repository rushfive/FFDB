using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.DbProviders.Mongo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo.DatabaseContext
{
	public class DbContext : DbContextBase//, IDatabaseContext
	{
		//public ITeamDatabaseContext Team { get; }
		//public IPlayerDatabaseContext Player { get; }
		//public IWeekStatsDatabaseContext Stats { get; }
		//public ILogDatabaseContext Log { get; }

		private Func<IMongoDatabase> _getDatabase { get; }
		private ILogger<DbContext> _logger { get; }

		public DbContext(
			Func<IMongoDatabase> getDatabase,
			ILoggerFactory loggerFactory)
			: base(getDatabase, loggerFactory)
		{
			//Team = new TeamDbContext(getDatabase, loggerFactory);
			//Player = new PlayerDbContext(getDatabase, loggerFactory);
			//Stats = new WeekStatsDbContext(getDatabase, loggerFactory);
			//Log = new LogDbContext(getDatabase, loggerFactory);

			_getDatabase = getDatabase;
			_logger = loggerFactory.CreateLogger<DbContext>();
		}

		public async Task InitializeAsync(bool force)
		{
			IMongoDatabase db = _getDatabase();

			bool alreadyInitialized = await HasBeenInitializedAsync();
			if (alreadyInitialized)
			{
				_logger.LogInformation("FFDB has already been initialized in database.");
				await OnAlreadyInitializedAsync(force, db);
			}

			// create collections and indexes
			List<Type> collectionTypes = CollectionResolver.GetDocumentTypes();
			foreach(var type in collectionTypes)
			{
				var name = CollectionNames.GetForType(type);
				await db.CreateCollectionAsync(name);

				await CollectionIndexes.CreateForTypeAsync(type, db);
			}

			_logger.LogInformation("Successfully initialized FFDB.");
		}

		private async Task OnAlreadyInitializedAsync(bool force, IMongoDatabase db)
		{
			if (!force)
			{
				throw new InvalidOperationException("FFDB is already initialized thus initialization cannot be run. "
					+ "You can force a re-initialization by setting the 'force' flag (this will clear all existing ffdb data).");
			}

			_logger.LogInformation("Re-initializing FFDB in database (due to 'force' flag being set).");
			
			var existingNames = await db.ListCollectionNames().ToListAsync();

			var existingFfdbCollections = existingNames
				.Where(n => n.StartsWith(CollectionConstants.FfdbPrefix))
				.ToList();

			_logger.LogInformation($"Found {existingFfdbCollections.Count} existing FFDB collections to drop.");

			foreach(var collection in existingFfdbCollections)
			{
				await db.DropCollectionAsync(collection);
				_logger.LogInformation($"Dropped collection '{collection}'.");
			}
		}

		public async Task<bool> HasBeenInitializedAsync()
		{
			List<string> ffdbCollectionNames = CollectionNames.GetAll();

			var existingNames = await _getDatabase().ListCollectionNames().ToListAsync();

			return existingNames.Any(n => ffdbCollectionNames.Contains(n));
		}
	}
}
