using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Components;
using R5.FFDB.Core.Database;
using R5.FFDB.DbProviders.PostgreSql.DatabaseContext;
using R5.Internals.PostgresMapper;
using System;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseProvider
{
	public class PostgresDbProvider : IDatabaseProvider
	{
		private PostgresConfig _config { get; }
		private IAppLogger _logger { get; }

		public PostgresDbProvider(
			PostgresConfig config,
			IAppLogger logger)
		{
			_config = config;
			_logger = logger;
		}

		public IDatabaseContext GetContext()
		{
			var dbConnection = new DbConnection(GetConnection);
			return new DbContext(dbConnection, _logger);
		}

		// todo: use connection builder
		private NpgsqlConnection GetConnection()
		{
			string connectionString = $"Host={_config.Host};Database={_config.DatabaseName};";

			if (_config.IsSecured)
			{
				connectionString += $"Username={_config.Username};Password={_config.Password}";
			}

			return new NpgsqlConnection(connectionString);
		}
	}
}
