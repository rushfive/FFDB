using Npgsql;
using R5.Internals.Extensions.Collections;
using R5.Internals.PostgresMapper.SqlBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace R5.Internals.PostgresMapper.QueryCommand
{
	public class CreateTableCommand<TEntity>
	{
		private Func<NpgsqlConnection> _getConnection { get; }
		private ConcatSqlBuilder _sqlBuilder { get; } = new ConcatSqlBuilder();

		public CreateTableCommand(
			Func<NpgsqlConnection> getConnection)
		{
			_getConnection = getConnection ?? throw new ArgumentNullException(nameof(getConnection));
			BuildCreateCommand();
		}

		private void BuildCreateCommand()
		{
			List<string> columnDefinitions = GetColumnDefinitions();

			_sqlBuilder
				.Append($"CREATE TABLE {MetadataResolver.TableName<TEntity>()} ")
				.Append($" ({string.Join(", ", columnDefinitions)}) ");
		}

		private List<string> GetColumnDefinitions()
		{
			List<string> definitions = MetadataResolver.TableColumns<TEntity>()
				.Select(c => c.DefinitionForCreateTable())
				.ToList();

			List<string> compositeKeys = MetadataResolver.CompositePrimaryKeys<TEntity>();
			if (!compositeKeys.IsNullOrEmpty())
			{
				definitions.Add(
					$"PRIMARY KEY({string.Join(", ", compositeKeys)})");
			}

			return definitions;
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
