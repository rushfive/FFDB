using Microsoft.Extensions.Logging;
using Npgsql;
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

		protected PostgresDbContext GetPostgresDbContext()
		{
			return new PostgresDbContext(
				_loggerFactory.CreateLogger<PostgresDbContext>(),
				_getConnection);
		}
		
		//public async Task ExecuteNonQueryAsync(string sqlCommand, List<(string key, string value)> parameters = null)
		//{
		//	using (NpgsqlConnection connection = _getConnection())
		//	{
		//		await connection.OpenAsync();

		//		using (var command = new NpgsqlCommand())
		//		{
		//			command.Connection = connection;
		//			command.CommandText = sqlCommand;

		//			if (parameters?.Any() ?? false)
		//			{
		//				parameters.ForEach(p => command.Parameters.AddWithValue(p.key, p.value));
		//			}

		//			await command.ExecuteNonQueryAsync();
		//		}
		//	}
		//}

		//public async Task ExecuteReaderAsync(string sqlCommand, Action<NpgsqlDataReader> readerCallback)
		//{
		//	using (NpgsqlConnection connection = _getConnection())
		//	{
		//		await connection.OpenAsync();

		//		using (var command = new NpgsqlCommand())
		//		{
		//			command.Connection = connection;
		//			command.CommandText = sqlCommand;

		//			using (NpgsqlDataReader reader = command.ExecuteReader())
		//			{
		//				readerCallback?.Invoke(reader);
		//			}
		//		}
		//	}
		//}

		//public async Task<TReturn> ExecuteReaderAsync<TReturn>(string sqlCommand, Func<NpgsqlDataReader, TReturn> onReadMapper)
		//{
		//	if (onReadMapper == null)
		//	{
		//		throw new ArgumentNullException(nameof(onReadMapper), "On-read mapper callback must be provided to execute a command with returning value.");
		//	}

		//	using (NpgsqlConnection connection = _getConnection())
		//	{
		//		await connection.OpenAsync();

		//		using (var command = new NpgsqlCommand())
		//		{
		//			command.Connection = connection;
		//			command.CommandText = sqlCommand;

		//			using (NpgsqlDataReader reader = command.ExecuteReader())
		//			{
		//				return onReadMapper.Invoke(reader);
		//			}
		//		}
		//	}
		//}

		//public Task<List<TSqlEntity>> SelectAsync<TSqlEntity>(string sqlCommand = null)
		//	where TSqlEntity : SqlEntity, new()
		//{
		//	if (string.IsNullOrWhiteSpace(sqlCommand))
		//	{
		//		sqlCommand = $"SELECT * FROM {EntityMetadata.TableName(typeof(TSqlEntity))}";
		//	}

		//	return ExecuteReaderAsync<List<TSqlEntity>>(sqlCommand, SqlEntityMapper.SelectAsEntitiesAsync<TSqlEntity>);
		//}

		//public Task ExecuteTransactionWrappedAsync(IEnumerable<string> commands)
		//{
		//	string sqlCommand = "BEGIN;" + Environment.NewLine;

		//	foreach(string command in commands)
		//	{
		//		sqlCommand += command;
		//	}

		//	sqlCommand += Environment.NewLine + "COMMIT;";

		//	return ExecuteNonQueryAsync(sqlCommand);
		//}

		//public async Task<bool> ExecuteAsBoolAsync(string sqlCommand)
		//{
		//	using (NpgsqlConnection connection = _getConnection())
		//	{
		//		await connection.OpenAsync();

		//		using (var command = new NpgsqlCommand())
		//		{
		//			command.Connection = connection;
		//			command.CommandText = sqlCommand;

		//			object result = await command.ExecuteScalarAsync();
		//			try
		//			{
		//				return Convert.ToBoolean(result);
		//			}
		//			catch (Exception ex)
		//			{
		//				_loggerFactory.CreateLogger<DbContextBase>()
		//					.LogError(ex, "Failed to convert returned object to bool.");
		//				throw;
		//			}
		//		}
		//	}
		//}

		protected ILogger<T> GetLogger<T>()
		{
			return _loggerFactory.CreateLogger<T>();
		}
	}
}
