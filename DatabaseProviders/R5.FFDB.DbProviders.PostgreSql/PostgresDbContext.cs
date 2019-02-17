using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.PostgreSql
{
	public class PostgresDbContext
	{
		private ILogger<PostgresDbContext> _logger { get; }
		private Func<NpgsqlConnection> _getConnection { get; }

		public PostgresDbContext(
			ILogger<PostgresDbContext> logger,
			Func<NpgsqlConnection> getConnection)
		{
			_logger = logger;
			_getConnection = getConnection;
		}

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

		public Task<List<TSqlEntity>> SelectAsync<TSqlEntity>(string sqlCommand = null)
			where TSqlEntity : SqlEntity, new()
		{
			if (string.IsNullOrWhiteSpace(sqlCommand))
			{
				sqlCommand = $"SELECT * FROM {EntityMetadata.TableName(typeof(TSqlEntity))}";
			}

			return QueryAsync<List<TSqlEntity>>(sqlCommand, SqlEntityMapper.SelectAsEntitiesAsync<TSqlEntity>);
		}

		private async Task<TReturn> QueryAsync<TReturn>(string sqlCommand, Func<NpgsqlDataReader, TReturn> onReadMapper)
		{
			if (onReadMapper == null)
			{
				throw new ArgumentNullException(nameof(onReadMapper), "On-read mapper callback must be provided to execute a command with returning value.");
			}

			using (NpgsqlConnection connection = _getConnection())
			{
				await connection.OpenAsync();

				using (var command = new NpgsqlCommand())
				{
					command.Connection = connection;
					command.CommandText = sqlCommand;

					using (NpgsqlDataReader reader = command.ExecuteReader())
					{
						return onReadMapper.Invoke(reader);
					}
				}
			}
		}

		public async Task<bool> QueryBoolAsync(string sqlCommand)
		{
			using (NpgsqlConnection connection = _getConnection())
			{
				await connection.OpenAsync();

				using (var command = new NpgsqlCommand())
				{
					command.Connection = connection;
					command.CommandText = sqlCommand;

					object result = await command.ExecuteScalarAsync();
					try
					{
						return Convert.ToBoolean(result);
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Failed to convert returned object to bool.");
						throw;
					}
				}
			}
		}
	}
}
