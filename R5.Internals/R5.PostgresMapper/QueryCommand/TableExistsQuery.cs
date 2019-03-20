using Npgsql;
using R5.Internals.PostgresMapper.SqlBuilders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.Internals.PostgresMapper.QueryCommand
{
	public class TableExistsQuery<TEntity>
	{
		private Func<NpgsqlConnection> _getConnection { get; }
		private ConcatSqlBuilder _sqlBuilder { get; } = new ConcatSqlBuilder();

		public TableExistsQuery(Func<NpgsqlConnection> getConnection)
		{
			_getConnection = getConnection ?? throw new ArgumentNullException(nameof(getConnection));

			var fullTableIdentifier = MetadataResolver.TableName<TEntity>();

			var split = fullTableIdentifier.Split('.');
			var schema = split[0];
			var tableName = split[1];

			var innerQuery = "SELECT 1 FROM pg_tables "
				+ $"WHERE schemaname = '{schema}' AND tablename = '{tableName}'";

			_sqlBuilder.Append($"SELECT EXISTS({innerQuery})");
		}

		public string GetSqlCommand()
		{
			return _sqlBuilder.GetResult();
		}

		public Task<bool> ExecuteAsync()
		{
			var sqlCommand = GetSqlCommand();
#if DEBUG
			DebugUtil.OutputSqlCommand(sqlCommand);
#endif
			NpgsqlConnection connection = _getConnection();
			return connection.ExecuteReaderAsync(sqlCommand, GetResultFromReader);
		}

		private static bool GetResultFromReader(NpgsqlDataReader reader)
		{
			while (reader.Read())
			{
				return (bool)reader.GetValue(0);
			}

			throw new InvalidOperationException("Should return or throw before this (har har)");
		}
	}
}
