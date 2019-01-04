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
		protected Func<NpgsqlConnection> _getConnection { get; }
		protected ILoggerFactory _loggerFactory { get; }

		protected DbContextBase(
			Func<NpgsqlConnection> getConnection,
			ILoggerFactory loggerFactory)
		{
			_getConnection = getConnection;
			_loggerFactory = loggerFactory;
		}

		// todo transactions
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

	// todo: move
	public static class SqlEntityMapper
	{
		public static List<T> SelectAsEntitiesAsync<T>(NpgsqlDataReader reader)
			where T : SqlEntity, new()
		{
			var result = new List<T>();

			List<ColumnInfo> columnInfos = EntityInfoMap.ColumnInfos(typeof(T));
			Dictionary<string, ColumnInfo> columnMap = columnInfos.ToDictionary(i => i.Name, i => i);

			while (reader.Read())
			{
				var entity = new T();

				for (int i = 0; i < reader.FieldCount; i++)
				{
					string columnName = reader.GetName(i);

					if (columnMap.TryGetValue(columnName, out ColumnInfo info))
					{
						object columnValue = reader.GetValue(i);

						if (info.Property.PropertyType.IsNullable())
						{
							SetValueForNullable(entity, columnValue, info);
						}
						else
						{
							SetValueForNonNullable(entity, columnValue, info);
						}
					}
				}

				result.Add(entity);
			}

			return result;
		}

		private static void SetValueForNullable(object entity, object columnValue, ColumnInfo info)
		{
			if (columnValue == null || columnValue.GetType() == typeof(DBNull))
			{
				info.Property.SetValue(entity, null);
				return;
			}

			Type type = info.Property.PropertyType;

			if (type.IsNullableEnum())
			{
				Type nullableType = Nullable.GetUnderlyingType(type);
				info.Property.SetValue(entity, Enum.Parse(nullableType, (string)columnValue));
				return;
			}

			info.Property.SetValue(entity, columnValue);
		}

		private static void SetValueForNonNullable(object entity, object columnValue, ColumnInfo info)
		{
			Type type = info.Property.PropertyType;

			object convertedValue = columnValue;
			switch (info.DataType)
			{
				case PostgresDataType.TEXT:
					if (type.IsEnum)
					{
						convertedValue = Enum.Parse(type, (string)columnValue);
					}
					break;
				case PostgresDataType.DATE:
				case PostgresDataType.TIMESTAMPTZ:
					DateTime dt = (DateTime)columnValue;
					dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);

					DateTimeOffset dtOffset = dt;
					convertedValue = dtOffset;
					break;
				case PostgresDataType.UUID:
				case PostgresDataType.INT:
				case PostgresDataType.FLOAT8:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(info.DataType), $"'{info.DataType}' is an invalid '{nameof(PostgresDataType)}'.");
			}

			info.Property.SetValue(entity, convertedValue);
		}
	}
}
