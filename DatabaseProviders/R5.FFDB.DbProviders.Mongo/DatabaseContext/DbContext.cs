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
		
		private ILogger<DbContext> _logger { get; }

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
			
			_logger = loggerFactory.CreateLogger<DbContext>();
		}

		
		public async Task InitializeAsync()
		{
			IMongoDatabase db = GetDatabase();

			List<Type> missingCollections = await GetMissingCollectionTypesAsync(db);
			foreach (var type in missingCollections)
			{
				var name = CollectionNames.GetForType(type);
				await db.CreateCollectionAsync(name);

				await CollectionIndexes.CreateForTypeAsync(type, db);
			}

			_logger.LogInformation("Successfully initialized FFDB.");
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

		public async Task<bool> HasBeenInitializedAsync()
		{
			List<string> ffdbCollectionNames = CollectionNames.GetAll();

			var existingNames = await GetDatabase().ListCollectionNames().ToListAsync();

			return existingNames.Any(n => ffdbCollectionNames.Contains(n));
		}
	}
}
