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
	public class LogDbContext : DbContextBase, ILogDatabaseContext
	{
		public LogDbContext(
			Func<IMongoDatabase> getDatabase,
			ILoggerFactory loggerFactory)
			: base(getDatabase, loggerFactory)
		{
		}

		public Task AddUpdateForWeekAsync(WeekInfo week)
		{
			throw new NotImplementedException();
		}

		public Task<List<WeekInfo>> GetUpdatedWeeksAsync()
		{
			throw new NotImplementedException();
		}

		public Task RemoveAllAsync()
		{
			throw new NotImplementedException();
		}

		public Task RemoveForWeekAsync(WeekInfo week)
		{
			throw new NotImplementedException();
		}
	}
}
