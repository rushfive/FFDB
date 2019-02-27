using R5.Internals.Extensions.Reflection;
using R5.Internals.PostgresMapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace R5.Internals.PostgresMapper.SqlBuilders
{
	public class SelectBuilder<TEntity>
		where TEntity : SqlEntity
	{
		private readonly ConcatSqlBuilder _selectBuilder = new ConcatSqlBuilder();

		public SelectBuilder(List<Expression<Func<TEntity, object>>> propertySelections)
		{
			var table = MetadataResolver.TableName<TEntity>();
			var selections = GetSelectionsFromProperties(propertySelections);

			string selectFrom = $"SELECT {selections} FROM {table}";
			_selectBuilder.Append(selectFrom);
		}

		private static string GetSelectionsFromProperties(List<Expression<Func<TEntity, object>>> propertySelections)
		{
			string selections;
			if (propertySelections == null)
			{
				selections = "*";
			}
			else
			{
				Dictionary<string, TableColumn> propertyColumnMap = MetadataResolver.PropertyColumnMap<TEntity>();

				var columns = new List<string>();
				foreach (Expression<Func<TEntity, object>> propExpression in propertySelections)
				{
					PropertyInfo property = propExpression.GetProperty();

					if (!propertyColumnMap.TryGetValue(property.Name, out TableColumn column))
					{
						throw new ArgumentException($"Property '{property.Name}' derived from expression is invalid: failed to find matching table column.");
					}

					columns.Add(column.Name);
				}

				selections = string.Join(", ", columns);
			}

			return selections;
		}

		public SelectBuilder<TEntity> Where(Expression<Func<TEntity, bool>> conditionExpression)
		{
			if (conditionExpression == null)
			{
				throw new ArgumentNullException("Condition expression must be provided. To select all columns, dont invoke this method.");
			}

			string whereCondition = WhereConditionBuilder<TEntity>.FromExpression(conditionExpression);
			string where = $"WHERE {whereCondition}";

			_selectBuilder.Append(where);

			return this;
		}

		public string CreateSql()
		{
			return _selectBuilder.GetResult();
		}

		public override string ToString()
		{
			return "SelectBuilder SQL:" + Environment.NewLine + _selectBuilder;
		}
	}
}
