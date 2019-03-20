using Npgsql;
using R5.Internals.Extensions.Collections;
using R5.Internals.PostgresMapper.Mappers;
using R5.Internals.PostgresMapper.Models;
using R5.Internals.PostgresMapper.SqlBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.Internals.PostgresMapper.QueryCommand
{
	public class InsertCommand<TEntity>
	{
		private Func<NpgsqlConnection> _getConnection { get; }
		private ConcatSqlBuilder _sqlBuilder { get; } = new ConcatSqlBuilder();

		public InsertCommand(
			Func<NpgsqlConnection> getConnection,
			List<TEntity> entities)
		{
			_getConnection = getConnection ?? throw new ArgumentNullException(nameof(getConnection));

			if (entities.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(entities), "At least one entity must be provided for an Insert command.");
			}

			List<TableColumn> columns = MetadataResolver.TableColumns<TEntity>();

			AppendInsertInto(columns);
			AppendEntityValues(entities, columns);
		}

		private void AppendInsertInto(List<TableColumn> columns)
		{
			var table = MetadataResolver.TableName<TEntity>();

			var columnNames = string.Join(", ", columns.Select(c => c.Name));

			_sqlBuilder.Append($"INSERT INTO {table} ({columnNames})");
		}

		private void AppendEntityValues(List<TEntity> entities, List<TableColumn> columns)
		{
			var entityValues = entities
				.Select(e => GetEntityDbValues(e, columns));

			var joined = string.Join(", ", entityValues);

			_sqlBuilder.Append($"VALUES {joined}");
		}

		private static string GetEntityDbValues(TEntity entity, List<TableColumn> columns)
		{
			List<string> entityDbValues = columns
				.Select(c => c.GetDbValueString(entity))
				.ToList();

			var joined = string.Join(", ", entityDbValues);

			return $"({joined})";
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
