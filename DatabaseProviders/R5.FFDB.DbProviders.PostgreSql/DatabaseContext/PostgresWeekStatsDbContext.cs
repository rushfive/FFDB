using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Core.Models;
using R5.FFDB.Database;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public class PostgresWeekStatsDbContext : PostgresDbContextBase, IWeekStatsDatabaseContext
	{
		public PostgresWeekStatsDbContext(
			Func<NpgsqlConnection> getConnection,
			ILoggerFactory loggerFactory)
			: base(getConnection, loggerFactory)
		{
		}

		public Task<List<WeekInfo>> GetExistingWeeksAsync()
		{
			throw new NotImplementedException();
		}

		public Task UpdateWeeksAsync(List<WeekStats> stats, bool overrideExisting)
		{
			throw new NotImplementedException();
		}
	}
}
