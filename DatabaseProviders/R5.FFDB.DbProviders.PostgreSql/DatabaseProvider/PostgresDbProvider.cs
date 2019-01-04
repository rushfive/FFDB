using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Database;
using R5.FFDB.DbProviders.PostgreSql.DatabaseContext;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseProvider
{
	public class PostgresDbProvider : IDatabaseProvider
	{
		private PostgresConfig _config { get; }
		private ILoggerFactory _loggerFactory { get; }

		public PostgresDbProvider(
			PostgresConfig config,
			ILoggerFactory loggerFactory)
		{
			_config = config;
			_loggerFactory = loggerFactory;
		}

		public IDatabaseContext GetContext()
		{
			return new DbContext(GetConnection, _loggerFactory);
		}

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
