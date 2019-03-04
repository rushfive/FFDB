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
			propertySelections?.ForEach(ValidatePropertyExpression);

			var table = MetadataResolver.TableName<TEntity>();
			var selections = GetSelectionsFromProperties(propertySelections);

			string selectFrom = $"SELECT {selections} FROM {table}";
			_sqlBuilder.Append(selectFrom);
		}

		// expected to be a lambda, with the body being a simple MemberAccess
		// todo: any way to get compile time checks on these??
		private static void ValidatePropertyExpression<TProp>(Expression<Func<TEntity, TProp>> expression)
		{
			var lambda = expression as LambdaExpression;
			if (lambda == null)
			{
				throw new Exception("NotLambda");
			}

			MemberExpression member;

			// if TProp is object, handle conversions from the actual underlying type
			if (lambda.Body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
			{
				member = unary.Operand as MemberExpression;
				if (member?.NodeType != ExpressionType.MemberAccess)
				{
					throw new ArgumentException($"Convert's operand type must be a '{ExpressionType.MemberAccess}'.");
				}
			}
			else
			{
				member = lambda.Body as MemberExpression;
				if (member?.NodeType != ExpressionType.MemberAccess)
				{
					throw new ArgumentException($"Lambda's body type must be a '{ExpressionType.MemberAccess}'. "
						+ "Don't reference properties of type 'object' (converting/casting is unsupported).");
				}
			}

			Type entityType = lambda.Parameters.Single().Type;

			if (member.Member.DeclaringType != entityType)
			{
				throw new ArgumentException($"Expression must reference a property from class '{entityType.Name}");
			}
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
