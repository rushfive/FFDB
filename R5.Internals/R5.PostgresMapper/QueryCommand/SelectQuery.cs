using Npgsql;
using R5.Internals.Extensions.Reflection;
using R5.Internals.PostgresMapper.Models;
using R5.Internals.PostgresMapper.SqlBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace R5.Internals.PostgresMapper.QueryCommand
{
	// todo internal
	public class SelectQuery<TEntity>
		where TEntity : new()
	{
		private Func<NpgsqlConnection> _getConnection { get; }
		private ConcatSqlBuilder _sqlBuilder { get; } = new ConcatSqlBuilder();

		public SelectQuery(
			Func<NpgsqlConnection> getConnection,
			List<Expression<Func<TEntity, object>>> propertySelections)
		{
			_getConnection = getConnection ?? throw new ArgumentNullException(nameof(getConnection));
			AppendSelectFrom(propertySelections);
		}

		private void AppendSelectFrom(List<Expression<Func<TEntity, object>>> propertySelections)
		{
			propertySelections?.ForEach(PropertySelectionResolver.ValidatePropertyExpression);

			var table = MetadataResolver.TableName<TEntity>();
			var selections = PropertySelectionResolver.GetSelectionsFromProperties(propertySelections);

			string selectFrom = $"SELECT {selections} FROM {table}";
			_sqlBuilder.Append(selectFrom);
		}

		public SelectQuery<TEntity> Where(Expression<Func<TEntity, bool>> conditionExpression)
		{
			if (conditionExpression == null)
			{
				throw new ArgumentNullException("Condition expression must be provided.");
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

		public Task<List<TEntity>> ExecuteAsync()
		{
			var sqlCommand = GetSqlCommand();
#if DEBUG
			Console.WriteLine(sqlCommand);
#endif
			NpgsqlConnection connection = _getConnection();
			return connection.ExecuteReaderAsync(sqlCommand, MapQueryToEntities);
		}

		private static List<TEntity> MapQueryToEntities(NpgsqlDataReader reader)
		{
			var result = new List<TEntity>();

			List<TableColumn> columns = MetadataResolver.TableColumns<TEntity>();
			Dictionary<string, TableColumn> columnMap = columns.ToDictionary(i => i.Name, i => i);

			while (reader.Read())
			{
				var entity = new TEntity();

				for (int i = 0; i < reader.FieldCount; i++)
				{
					string columnName = reader.GetName(i);

					if (columnMap.TryGetValue(columnName, out TableColumn column))
					{
						object columnValue = reader.GetValue(i);
						column.SetValue(entity, columnValue);
					}
					else
					{
						Console.ForegroundColor = ConsoleColor.Yellow;
						Console.WriteLine($"Warning: Failed to map db column '{columnName}' a TableColumn on entity '{typeof(TEntity)}' (did the schema change recently? is a db upgrade required?)");
						Console.ResetColor();
					}
				}

				result.Add(entity);
			}

			return result;
		}

		public override string ToString()
		{
			return "SelectBuilder SQL:" + Environment.NewLine + _sqlBuilder;
		}
	}
}
