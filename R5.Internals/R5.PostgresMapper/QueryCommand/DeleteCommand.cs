using Npgsql;
using R5.Internals.PostgresMapper.SqlBuilders;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.Internals.PostgresMapper.QueryCommand
{
	public class DeleteCommand<TEntity>
	{
		private Func<NpgsqlConnection> _getConnection { get; }
		private ConcatSqlBuilder _sqlBuilder { get; } = new ConcatSqlBuilder();

		public DeleteCommand(
			Func<NpgsqlConnection> getConnection,
			List<TEntity> entities)
		{
			_getConnection = getConnection ?? throw new ArgumentNullException(nameof(getConnection));

			AppendDeleteFrom();
		}

		private void AppendDeleteFrom()
		{
			var tableName = MetadataResolver.TableName<TEntity>();
			_sqlBuilder.Append($"DELETE FROM {tableName}");
		}
	}
}
