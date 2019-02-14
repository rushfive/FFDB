using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using R5.FFDB.Core.Database;
using R5.FFDB.DbProviders.Mongo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo.DatabaseContext
{
	public class DbContext : DbContextBase, IDatabaseContext
	{
		public IPlayerDbContext Player { get; }
		public IPlayerStatsDbContext PlayerStats { get; }
		public ITeamDbContext Team { get; }
		public ITeamStatsDbContext TeamStats { get; }
		public IUpdateLogDbContext UpdateLog { get; }
		public IWeekMatchupsDbContext WeekMatchups { get; }

		public DbContext(
			Func<IMongoDatabase> getDatabase,
			ILoggerFactory loggerFactory)
			: base(getDatabase, loggerFactory)
		{
			Player = new PlayerDbContext(getDatabase, loggerFactory);
			PlayerStats = new PlayerStatsDbContext(getDatabase, loggerFactory);
			Team = new TeamDbContext(getDatabase, loggerFactory);
			TeamStats = new TeamStatsDbContext(getDatabase, loggerFactory);
			UpdateLog = new UpdateLogDbContext(getDatabase, loggerFactory);
			WeekMatchups = new WeekMatchupsDbContext(getDatabase, loggerFactory);
		}

		
		public async Task InitializeAsync()
		{
			var logger = GetLogger<DbContext>();

			logger.LogDebug("Initializing FFDB - creating required collections.");

			IMongoDatabase db = GetDatabase();

			List<Type> missingCollections = await GetMissingCollectionTypesAsync(db);
			foreach (var type in missingCollections)
			{
				await CreateCollectionAsync(type, db);
			}

			logger.LogInformation("Successfully initialized FFDB.");
		}

		private async Task<List<Type>> GetMissingCollectionTypesAsync(IMongoDatabase db)
		{
			HashSet<string> existing = (await db.ListCollectionNames().ToListAsync())
				.Where(n => n.StartsWith(CollectionConstants.FfdbPrefix))
				.ToHashSet();

			return CollectionResolver.GetDocumentTypes()
				.Where(t => !existing.Contains(
					CollectionNames.GetForType(t)))
				.ToList();
		}

		private async Task CreateCollectionAsync(Type collectionType, IMongoDatabase db)
		{
			var name = CollectionNames.GetForType(collectionType);
			await db.CreateCollectionAsync(name);

			await CollectionIndexes.CreateForTypeAsync(collectionType, db);
		}

		public async Task<bool> HasBeenInitializedAsync()
		{
			List<string> ffdbCollectionNames = CollectionNames.GetAll();

			var existingNames = await GetDatabase().ListCollectionNames().ToListAsync();

			return existingNames.Any(n => ffdbCollectionNames.Contains(n));
		}
	}
}
