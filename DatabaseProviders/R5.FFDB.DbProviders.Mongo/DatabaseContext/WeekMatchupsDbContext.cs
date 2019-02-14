using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.Mongo.DatabaseContext
{
	public class WeekMatchupsDbContext : DbContextBase, IWeekMatchupsDbContext
	{
		public WeekMatchupsDbContext(
			Func<IMongoDatabase> getDatabase,
			ILoggerFactory loggerFactory)
			: base(getDatabase, loggerFactory)
		{
		}

		public Task<List<WeekMatchup>> GetAsync(WeekInfo week)
		{
			throw new NotImplementedException();
		}

		public Task AddAsync(List<WeekMatchup> matchups)
		{
			throw new NotImplementedException();
		}
	}
}
