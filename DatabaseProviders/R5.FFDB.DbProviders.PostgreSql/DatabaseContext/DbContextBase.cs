using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Components;
using R5.FFDB.DbProviders.PostgreSql.Models;
using R5.FFDB.DbProviders.PostgreSql.Models.ColumnInfos;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public abstract class DbContextBase
	{
		private Func<NpgsqlConnection> _getConnection { get; }
		private ILoggerFactory _loggerFactory { get; }

		protected DbContextBase(
			Func<NpgsqlConnection> getConnection,
			ILoggerFactory loggerFactory)
		{
			_getConnection = getConnection;
			_loggerFactory = loggerFactory;
		}
		
		public async Task ExecuteNonQueryAsync(string sqlCommand, List<(string key, string value)> parameters = null)
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

		public async Task ExecuteReaderAsync(string sqlCommand, Action<NpgsqlDataReader> readerCallback)
		{
			using (NpgsqlConnection connection = _getConnection())
			{
				await connection.OpenAsync();

				using (var command = new NpgsqlCommand())
				{
					command.Connection = connection;
					command.CommandText = sqlCommand;

					using (NpgsqlDataReader reader = command.ExecuteReader())
					{
						readerCallback?.Invoke(reader);
					}
				}
			}
		}

		public async Task<TReturn> ExecuteReaderAsync<TReturn>(string sqlCommand, Func<NpgsqlDataReader, TReturn> readerCallback)
		{
			if (readerCallback == null)
			{
				throw new ArgumentNullException(nameof(readerCallback), "Npgsql reader callback must be provided to execute a command with returning value.");
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
						return readerCallback.Invoke(reader);
					}
				}
			}
		}

		public Task<List<TSqlEntity>> SelectAsEntitiesAsync<TSqlEntity>(string sqlCommand = null)
			where TSqlEntity : SqlEntity, new()
		{
			if (string.IsNullOrWhiteSpace(sqlCommand))
			{
				sqlCommand = $"SELECT * FROM {EntityInfoMap.TableName(typeof(TSqlEntity))}";
			}

			return ExecuteReaderAsync<List<TSqlEntity>>(sqlCommand, SqlEntityMapper.SelectAsEntitiesAsync<TSqlEntity>);
		}

		public Task ExecuteTransactionWrappedAsync(IEnumerable<string> commands)
		{
			string sqlCommand = "BEGIN;" + Environment.NewLine;

			foreach(string command in commands)
			{
				sqlCommand += command;
			}

			sqlCommand += Environment.NewLine + "COMMIT;";

			return ExecuteNonQueryAsync(sqlCommand);
		}

		protected ILogger<T> GetLogger<T>()
		{
			return _loggerFactory.CreateLogger<T>();
		}
	}
}
