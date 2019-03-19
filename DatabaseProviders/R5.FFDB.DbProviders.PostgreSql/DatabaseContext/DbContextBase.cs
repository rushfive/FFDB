using Microsoft.Extensions.Logging;
using R5.FFDB.Components;
using R5.Internals.PostgresMapper;
using System;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public abstract class DbContextBase
	{
		protected DbConnection DbConnection { get; }
		protected IAppLogger Logger { get; }

		protected DbContextBase(
			DbConnection dbConnection,
			IAppLogger logger)
		{
			DbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}
	}
}
