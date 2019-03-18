using Microsoft.Extensions.Logging;
using R5.Internals.PostgresMapper;
using System;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public abstract class DbContextBase
	{
		protected DbConnection DbConnection { get; }
		protected ILogger<DbContextBase> Logger { get; }

		protected DbContextBase(
			DbConnection dbConnection,
			ILogger<DbContextBase> logger)
		{
			DbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}
	}
}
