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
	public class TeamStatsDbContext : DbContextBase, ITeamStatsDbContext
	{
		public TeamStatsDbContext(
			Func<IMongoDatabase> getDatabase,
			ILoggerFactory loggerFactory)
			: base(getDatabase, loggerFactory)
		{
		}

		public Task<List<TeamWeekStats>> GetAsync(WeekInfo week)
		{
			throw new NotImplementedException();
		}

		public Task AddAsync(List<TeamWeekStats> stats)
		{
			throw new NotImplementedException();
		}
	}
}
