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
	public class WeekStatsDbContext : DbContextBase, IWeekStatsDatabaseContext
	{
		public WeekStatsDbContext(
			Func<IMongoDatabase> getDatabase,
			ILoggerFactory loggerFactory)
			: base(getDatabase, loggerFactory)
		{
		}

		public Task RemoveAllAsync()
		{
			throw new NotImplementedException();
		}

		public Task RemoveForWeekAsync(WeekInfo week)
		{
			throw new NotImplementedException();
		}

		public Task UpdateWeekAsync(WeekStats stats)
		{
			throw new NotImplementedException();
		}

		public Task UpdateWeeksAsync(List<WeekStats> stats)
		{
			throw new NotImplementedException();
		}
	}
}
