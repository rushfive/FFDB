using Npgsql;
using R5.Internals.PostgresMapper.SqlBuilders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace R5.Internals.PostgresMapper.QueryCommand
{
	public class DeleteWhereCommand<TEntity> where TEntity : class
	{
		private Func<NpgsqlConnection> _getConnection { get; }
		private ConcatSqlBuilder _sqlBuilder { get; } = new ConcatSqlBuilder();

		public DeleteWhereCommand(
			Func<NpgsqlConnection> getConnection,
			Expression<Func<TEntity, bool>> conditionExpression)
		{
			_getConnection = getConnection ?? throw new ArgumentNullException(nameof(getConnection));
			if (conditionExpression == null)
			{
				throw new ArgumentNullException(nameof(conditionExpression), "Condition expression must be provided..");
			}

			AppendDeleteFrom();
			AppendWhereCondition(conditionExpression);
		}

		private void AppendDeleteFrom()
		{
			var tableName = MetadataResolver.TableName<TEntity>();
			_sqlBuilder.Append($"DELETE FROM {tableName}");
		}

		private void AppendWhereCondition(Expression<Func<TEntity, bool>> conditionExpression)
		{
			string whereCondition = WhereConditionBuilder<TEntity>.FromExpression(conditionExpression);
			string where = $"WHERE {whereCondition}";

			_sqlBuilder.Append(where);
		}

		public string GetSqlCommand()
		{
			return _sqlBuilder.GetResult();
		}

		public Task ExecuteAsync()
		{
			var sqlCommand = GetSqlCommand();
#if DEBUG
			DebugUtil.OutputSqlCommand(sqlCommand);
#endif
			NpgsqlConnection connection = _getConnection();
			return connection.ExecuteNonQueryAsync(sqlCommand);
		}
	}
}
