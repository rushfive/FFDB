using Npgsql;
using R5.Internals.PostgresMapper.Models;
using R5.Internals.PostgresMapper.QueryCommand;
using R5.Internals.PostgresMapper.SqlBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace R5.Internals.PostgresMapper
{
	public class DbConnection
	{
		private Func<NpgsqlConnection> _getConnection { get; }

		public DbConnection(Func<NpgsqlConnection> getConnection)
		{
			_getConnection = getConnection ?? throw new ArgumentNullException(nameof(getConnection));
		}

		public SelectQuery<TEntity> Select<TEntity>()
			where TEntity : new()
		{
			return new SelectQuery<TEntity>(this, null);
		}

		public SelectQuery<TEntity> Select<TEntity>(params Expression<Func<TEntity, object>>[] properties)
			where TEntity : new()
		{
			if (properties == null || properties.Length == 0)
			{
				throw new ArgumentNullException(nameof(properties), "Property expressions must be provided.");
			}

			return new SelectQuery<TEntity>(this, properties.ToList());
		}

		public ExistsQuery<TEntity> Exists<TEntity>()
		{
			return new ExistsQuery<TEntity>(this);
		}

		public async Task<TReturn> ExecuteReaderAsync<TReturn>(string sqlCommand, Func<NpgsqlDataReader, TReturn> onReadMapper)
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
	}
}
