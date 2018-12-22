using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Core.Models;
using R5.FFDB.Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public class PostgresPlayerDbContext : PostgresDbContextBase, IPlayerDatabaseContext
	{
		public PostgresPlayerDbContext(
			Func<NpgsqlConnection> getConnection,
			ILoggerFactory loggerFactory)
			: base(getConnection, loggerFactory)
		{
		}

		public Task<List<PlayerProfile>> GetExistingAsync()
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(List<PlayerProfile> players, bool overrideExisting)
		{
			throw new NotImplementedException();
		}
	}
}
