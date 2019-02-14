using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo.DatabaseContext
{
	public class UpdateLogDbContext : DbContextBase, IUpdateLogDbContext
	{
		public UpdateLogDbContext(
			Func<IMongoDatabase> getDatabase,
			ILoggerFactory loggerFactory)
			: base(getDatabase, loggerFactory)
		{
		}

		public Task<List<WeekInfo>> GetAsync()
		{
			throw new NotImplementedException();
		}

		public Task AddAsync(WeekInfo week)
		{
			throw new NotImplementedException();
		}

		public Task<bool> HasUpdatedWeekAsync(WeekInfo week)
		{
			throw new NotImplementedException();
		}
	}
}
