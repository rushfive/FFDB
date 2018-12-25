using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public abstract class PostgresDbContextBase
	{
		protected Func<NpgsqlConnection> _getConnection { get; }
		protected ILoggerFactory _loggerFactory { get; }

		protected PostgresDbContextBase(
			Func<NpgsqlConnection> getConnection,
			ILoggerFactory loggerFactory)
		{
			_getConnection = getConnection;
			_loggerFactory = loggerFactory;
		}

		// todo transactions
		public async Task ExecuteCommandAsync(string sqlCommand, List<(string key, string value)> parameters = null)
		{
			using (NpgsqlConnection connection = _getConnection())
			{
				await connection.OpenAsync();

				using (var command = new NpgsqlCommand())
				{
					command.Connection = connection;
					command.CommandText = sqlCommand;

					if (parameters?.Any() ?? false)
					{
						parameters.ForEach(p => command.Parameters.AddWithValue(p.key, p.value));
					}

					await command.ExecuteNonQueryAsync();
				}
			}
		}

		protected ILogger<T> GetLogger<T>()
		{
			return _loggerFactory.CreateLogger<T>();
		}
	}
}
