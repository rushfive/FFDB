//using Microsoft.Extensions.Logging;
//using Npgsql;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace R5.FFDB.DbProviders.PostgreSql
//{
//	public class PostgresDbContext
//	{
//		private ILogger<PostgresDbContext> _logger { get; }
//		private Func<NpgsqlConnection> _getConnection { get; }

//		public PostgresDbContext(
//			ILogger<PostgresDbContext> logger,
//			Func<NpgsqlConnection> getConnection)
//		{
//			_logger = logger;
//			_getConnection = getConnection;
//		}

//		//public async Task<List<TEntity>> SelectAsync<TEntity>(string sqlCommand = null)
//		//	where TEntity : SqlEntity, new()
//		//{
//		//	if (string.IsNullOrWhiteSpace(sqlCommand))
//		//	{
//		//		sqlCommand = $"SELECT * FROM {EntityMetadata.TableName(typeof(TEntity))}";
//		//	}
//		//	return null;
//		//	//return await QueryAsync(sqlCommand, SqlEntityMapper.SelectAsEntitiesAsync<TEntity>)
//		//	//	.ConfigureAwait(false);
//		//}

//		private async Task<TReturn> QueryAsync<TReturn>(string sqlCommand, Func<NpgsqlDataReader, TReturn> onReadMapper)
//		{
//			if (onReadMapper == null)
//			{
//				throw new ArgumentNullException(nameof(onReadMapper), "On-read mapper callback must be provided to execute a command with returning value.");
//			}

//			using (NpgsqlConnection connection = _getConnection())
//			{
//				await connection.OpenAsync();

//				using (var command = new NpgsqlCommand())
//				{
//					command.Connection = connection;
//					command.CommandText = sqlCommand;

//					using (NpgsqlDataReader reader = command.ExecuteReader())
//					{
//						return onReadMapper.Invoke(reader);
//					}
//				}
//			}
//		}

//		//public async Task TruncateAsync<TEntity>()
//		//	where TEntity : SqlEntity
//		//{
//		//	string tableName = EntityMetadata.TableName(typeof(TEntity));

//		//	string sqlCommand = $"TRUNCATE {tableName};";

//		//	await ExecuteCommandAsync(sqlCommand).ConfigureAwait(false);
//		//}

//		//public async Task InsertAsync<TEntity>(TEntity entity)
//		//	where TEntity : SqlEntity
//		//{
//		//	if (entity == null)
//		//	{
//		//		throw new ArgumentNullException(nameof(entity), "Entity must be provided to perform insert.");
//		//	}

//		//	string sqlCommand = SqlCommandBuilder.Rows.Insert(entity);

//		//	await ExecuteCommandAsync(sqlCommand).ConfigureAwait(false);
//		//}

//		//public async Task InsertManyAsync<TEntity>(IEnumerable<TEntity> entities)
//		//	where TEntity : SqlEntity
//		//{
//		//	if (entities == null || !entities.Any())
//		//	{
//		//		throw new ArgumentNullException(nameof(entities), "Entities must be provided to perform inserts.");
//		//	}

//		//	string sqlCommand = SqlCommandBuilder.Rows.InsertMany(entities);

//		//	await ExecuteCommandAsync(sqlCommand).ConfigureAwait(false);
//		//}


//		// OLD BELOW - check to make private!

//		public async Task ExecuteCommandAsync(string sqlCommand, List<(string key, string value)> parameters = null)
//		{
//			using (NpgsqlConnection connection = _getConnection())
//			{
//				await connection.OpenAsync();

//				using (var command = new NpgsqlCommand())
//				{
//					command.Connection = connection;
//					command.CommandText = sqlCommand;

//					if (parameters?.Any() ?? false)
//					{
//						parameters.ForEach(p => command.Parameters.AddWithValue(p.key, p.value));
//					}

//					await command.ExecuteNonQueryAsync().ConfigureAwait(false);
//				}
//			}
//		}



//		public async Task<bool> QueryBoolAsync(string sqlCommand)
//		{
//			using (NpgsqlConnection connection = _getConnection())
//			{
//				await connection.OpenAsync();

//				using (var command = new NpgsqlCommand())
//				{
//					command.Connection = connection;
//					command.CommandText = sqlCommand;

//					object result = await command.ExecuteScalarAsync();
//					try
//					{
//						return Convert.ToBoolean(result);
//					}
//					catch (Exception ex)
//					{
//						_logger.LogError(ex, "Failed to convert returned object to bool.");
//						throw;
//					}
//				}
//			}
//		}
//	}
//}
