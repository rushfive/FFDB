using Npgsql;
using R5.Internals.PostgresMapper.SqlBuilders;
using System;
using System.Threading.Tasks;

namespace R5.Internals.PostgresMapper.QueryCommand
{
	public class TruncateCommand<TEntity>
	{
		private Func<NpgsqlConnection> _getConnection { get; }
		private ConcatSqlBuilder _sqlBuilder { get; } = new ConcatSqlBuilder();

		public TruncateCommand(
			Func<NpgsqlConnection> getConnection)
		{
			_getConnection = getConnection ?? throw new ArgumentNullException(nameof(getConnection));
			BuildTruncateCommand();
		}

		private void BuildTruncateCommand()
		{
			_sqlBuilder
				.Append($"TRUNCATE {MetadataResolver.TableName<TEntity>()}");
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
