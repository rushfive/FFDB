using Npgsql;
using R5.Internals.PostgresMapper.SqlBuilders;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.Internals.PostgresMapper.QueryCommand
{
	public class InsertCommand<TEntity>
	{
		private Func<NpgsqlConnection> _getConnection { get; }
		private ConcatSqlBuilder _sqlBuilder { get; } = new ConcatSqlBuilder();

		public InsertCommand(
			Func<NpgsqlConnection> getConnection)
		{
			_getConnection = getConnection ?? throw new ArgumentNullException(nameof(getConnection));
			BuildTruncateCommand();
		}

		private void BuildTruncateCommand()
		{
			throw new NotImplementedException();
		}
	}
}
