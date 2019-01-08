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
	public class TeamDbContext : DbContextBase, ITeamDatabaseContext
	{
		public TeamDbContext(
			Func<IMongoDatabase> getDatabase,
			ILoggerFactory loggerFactory)
			: base(getDatabase, loggerFactory)
		{
		}

		public Task AddTeamsAsync()
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

		public Task UpdateGameStatsAsync(List<TeamWeekStats> stats)
		{
			throw new NotImplementedException();
		}

		public Task UpdateRostersAsync(List<Roster> rosters)
		{
			throw new NotImplementedException();
		}
	}
}
