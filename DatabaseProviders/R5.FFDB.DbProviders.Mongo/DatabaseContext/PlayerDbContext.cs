using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using R5.FFDB.Core.Models;
using R5.FFDB.Database.DbContext;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo.DatabaseContext
{
	public class PlayerDbContext : DbContextBase, IPlayerDatabaseContext
	{
		public PlayerDbContext(
			Func<IMongoDatabase> getDatabase,
			ILoggerFactory loggerFactory)
			: base(getDatabase, loggerFactory)
		{
		}

		public Task AddAsync(List<PlayerProfile> players, List<Roster> rosters)
		{
			throw new NotImplementedException();
		}

		public Task<List<PlayerProfile>> GetAllAsync()
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(List<PlayerProfile> players, List<Roster> rosters)
		{
			throw new NotImplementedException();
		}
	}
}
