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
	}
}
