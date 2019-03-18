using Npgsql;
using R5.Internals.PostgresMapper.SqlBuilders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.Internals.PostgresMapper.QueryCommand
{
	public class DeleteCommand<TEntity>
		where TEntity : class
	{
		private Func<NpgsqlConnection> _getConnection { get; }
		private ConcatSqlBuilder _sqlBuilder { get; } = new ConcatSqlBuilder();

		public DeleteCommand(
			Func<NpgsqlConnection> getConnection,
			TEntity entity)
		{
			_getConnection = getConnection ?? throw new ArgumentNullException(nameof(getConnection));
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity), "Entity to be deleted must be provided.");
			}

			AppendDeleteFrom();
			AppendWhereCondition(entity);
		}

		private void AppendDeleteFrom()
		{
			var tableName = MetadataResolver.TableName<TEntity>();
			_sqlBuilder.Append($"DELETE FROM {tableName}");
		}

		private void AppendWhereCondition(TEntity entity)
		{
			Func<TEntity, string> getKeyMatch = MetadataResolver.GetPrimaryKeyMatchConditionFunc<TEntity>();

			string condition = getKeyMatch(entity);

			_sqlBuilder.Append($"WHERE {condition}");
		}

		public string GetSqlCommand()
		{
			return _sqlBuilder.GetResult();
		}

		public Task ExecuteAsync()
		{
			var sqlCommand = GetSqlCommand();
#if DEBUG
			Console.WriteLine(sqlCommand);
#endif
			NpgsqlConnection connection = _getConnection();
			return connection.ExecuteNonQueryAsync(sqlCommand);
		}
	}
}
