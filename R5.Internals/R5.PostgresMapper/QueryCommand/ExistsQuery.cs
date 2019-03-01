using Npgsql;
using R5.Internals.PostgresMapper.SqlBuilders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace R5.Internals.PostgresMapper.QueryCommand
{
	// todo internal
	public class ExistsQuery<TEntity>
	{
		private Func<NpgsqlConnection> _getConnection { get; }
		private ConcatSqlBuilder _sqlBuilder { get; } = new ConcatSqlBuilder();

		public ExistsQuery(Func<NpgsqlConnection> getConnection)
		{
			_getConnection = getConnection ?? throw new ArgumentNullException(nameof(getConnection));

			var table = MetadataResolver.TableName<TEntity>();

			string selectFrom = $"SELECT * FROM {table}";
			_sqlBuilder.Append(selectFrom);
		}

		public ExistsQuery<TEntity> Where(Expression<Func<TEntity, bool>> conditionExpression)
		{
			if (conditionExpression == null)
			{
				throw new ArgumentNullException("Condition expression must be provided. To select all columns, dont invoke this method.");
			}

			string whereCondition = WhereConditionBuilder<TEntity>.FromExpression(conditionExpression);
			string where = $"WHERE {whereCondition}";

			_sqlBuilder.Append(where);

			return this;
		}

		public string GetSqlCommand()
		{
			return _sqlBuilder.GetResult();
		}

		public Task<bool> ExecuteAsync()
		{
			var sqlCommand = GetSqlCommand();
#if DEBUG
			Console.WriteLine(sqlCommand);
#endif
			NpgsqlConnection connection = _getConnection();
			return connection.ExecuteReaderAsync(sqlCommand, r => r.HasRows);
		}

		private static bool QueryHasRowResults(NpgsqlDataReader reader)
		{
			return reader.HasRows;
		}
	}
}
