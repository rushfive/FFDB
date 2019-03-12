using Npgsql;
using R5.Internals.PostgresMapper.QueryCommand;
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
			return new SelectQuery<TEntity>(_getConnection, null);
		}

		public SelectQuery<TEntity> Select<TEntity>(params Expression<Func<TEntity, object>>[] properties)
			where TEntity : new()
		{
			if (properties == null || properties.Length == 0)
			{
				throw new ArgumentNullException(nameof(properties), "Property expressions must be provided.");
			}

			return new SelectQuery<TEntity>(_getConnection, properties.ToList());
		}

		public UnionSelectQuery<TResult> UnionSelect<TResult>()
		{
			return new UnionSelectQuery<TResult>(_getConnection);
		}

		public InsertCommand<TEntity> Insert<TEntity>(TEntity entity)
		{
			return new InsertCommand<TEntity>(_getConnection, new List<TEntity> { entity });
		}

		public InsertCommand<TEntity> InsertMany<TEntity>(List<TEntity> entities)
		{
			return new InsertCommand<TEntity>(_getConnection, entities);
		}

		public UpdateCommand<TEntity> Update<TEntity>()
			where TEntity : class
		{
			return new UpdateCommand<TEntity>(_getConnection);
		}

		public DeleteCommand<TEntity> Delete<TEntity>(TEntity entity)
			where TEntity : class
		{
			return new DeleteCommand<TEntity>(_getConnection, entity);
		}

		public DeleteWhereCommand<TEntity> DeleteWhere<TEntity>(Expression<Func<TEntity, bool>> conditionExpression)
			where TEntity : class
		{
			return new DeleteWhereCommand<TEntity>(_getConnection, conditionExpression);
		}

		public ExistsQuery<TEntity> Exists<TEntity>()
		{
			return new ExistsQuery<TEntity>(_getConnection);
		}

		public CreateTableCommand<TEntity> CreateTable<TEntity>()
		{
			return new CreateTableCommand<TEntity>(_getConnection);
		}

		public TruncateCommand<TEntity> Truncate<TEntity>()
		{
			return new TruncateCommand<TEntity>(_getConnection);
		}

		public Task CreateSchema(string schema)
		{
			if (string.IsNullOrWhiteSpace(schema))
			{
				throw new ArgumentNullException(nameof(schema), "Schema must be provided.");
			}

			var sqlCommand = $"CREATE SCHEMA {schema};";

			return _getConnection().ExecuteNonQueryAsync(sqlCommand);
		}
	}
}
