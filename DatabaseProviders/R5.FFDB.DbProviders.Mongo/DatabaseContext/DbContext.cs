using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using R5.FFDB.Components;
using R5.FFDB.Core;
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
			IAppLogger logger)
			: base(getDatabase, logger)
		{
			Player = new PlayerDbContext(getDatabase, logger);
			PlayerStats = new PlayerStatsDbContext(getDatabase, logger);
			Team = new TeamDbContext(getDatabase, logger);
			TeamStats = new TeamStatsDbContext(getDatabase, logger);
			UpdateLog = new UpdateLogDbContext(getDatabase, logger);
			WeekMatchups = new WeekMatchupsDbContext(getDatabase, logger);
		}

		
		public async Task InitializeAsync()
		{
			Logger.LogInformation("Initializing FFDB - creating required collections.");

			IMongoDatabase db = GetDatabase();

			List<Type> missingCollections = await GetMissingCollectionTypesAsync(db);
			foreach (var type in missingCollections)
			{
				await CreateCollectionAsync(type, db);
			}

			Logger.LogInformation("Successfully initialized FFDB.");
		}

		private async Task<List<Type>> GetMissingCollectionTypesAsync(IMongoDatabase db)
		{
			HashSet<string> existing = (await db.ListCollectionNames().ToListAsync())
				.Where(n => n.StartsWith(Collection.FfdbPrefix))
				.ToHashSet();

			return CollectionResolver.GetDocumentTypes()
				.Where(t => !existing.Contains(
					CollectionResolver.GetName(t)))
				.ToList();
		}

		private async Task CreateCollectionAsync(Type collectionType, IMongoDatabase db)
		{
			var name = CollectionResolver.GetName(collectionType);
			await db.CreateCollectionAsync(name);

			await Indexes.CreateForTypeAsync(collectionType, db);
		}

		public async Task<bool> HasBeenInitializedAsync()
		{
			var requiredInitChecks = new List<Func<Task<bool>>>
			{
				async () =>
				{
					var existingCollections = (await GetDatabase().ListCollectionNames().ToListAsync()).ToHashSet(StringComparer.OrdinalIgnoreCase);
					return CollectionResolver.GetAllNames().All(n => existingCollections.Contains(n));
				},
				async () =>
				{
					var existingTeams = (await Team.GetExistingTeamIdsAsync()).ToHashSet();
					return Teams.GetAll().Select(t => t.Id).All(id => existingTeams.Contains(id));
				}
			};

			foreach(var checkFunc in requiredInitChecks)
			{
				if (!await checkFunc())
				{
					return false;
				}
			}

			return true;
		}
	}
}
