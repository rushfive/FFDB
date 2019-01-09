using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using R5.FFDB.Components.CoreData.TeamData.Models;
using R5.FFDB.Core.Models;
using R5.FFDB.Database.DbContext;
using R5.FFDB.DbProviders.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo.DatabaseContext
{
	public class TeamDbContext : DbContextBase, ITeamDatabaseContext
	{
		public TeamDbContext(
			Func<IMongoDatabase> getDatabase,
			ILoggerFactory loggerFactory)
			: base(getDatabase, loggerFactory)
		{
		}

		public async Task AddTeamsAsync()
		{
			ILogger<TeamDbContext> logger = GetLogger<TeamDbContext>();
			var collectionName = CollectionResolver.CollectionNameFor<TeamDocument>();

			logger.LogDebug($"Adding NFL team documents to '{collectionName}' collection.");

			List<TeamDocument> teamSqls = TeamDataStore
				.GetAll()
				.Select(TeamDocument.FromCoreEntity)
				.ToList();

			MongoDbContext context = GetMongoDbContext();
			await context.InsertManyAsync(teamSqls);

			logger.LogInformation($"Successfully added team documents to '{collectionName}' collection.");
		}

		public Task UpdateRostersAsync(List<Roster> rosters)
		{
			throw new NotImplementedException();
		}

		public Task UpdateGameStatsAsync(List<TeamWeekStats> stats)
		{
			throw new NotImplementedException();
		}

		public Task RemoveAllGameStatsAsync()
		{
			throw new NotImplementedException();
		}

		public Task RemoveGameStatsForWeekAsync(WeekInfo week)
		{
			throw new NotImplementedException();
		}
	}
}
