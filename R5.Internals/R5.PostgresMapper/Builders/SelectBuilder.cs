using R5.Internals.PostgresMapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace R5.Internals.PostgresMapper.Builders
{
	public static class Builders<TEntity>
		where TEntity : SqlEntity
	{
		public static SelectBuilder<TEntity> Select()
		{
			return new SelectBuilder<TEntity>(null);
		}

		public static SelectBuilder<TEntity> Select(params Expression<Func<TEntity, object>>[] properties)
		{
			if (properties == null || properties.Length == 0)
			{
				throw new ArgumentNullException(nameof(properties), "Property expressions must be provided.");
			}


			return new SelectBuilder<TEntity>(properties.ToList());
		}
	}





	public static class Query<TEntity>
		where TEntity : SqlEntity
	{
		
	}

	public class SelectBuilder<TEntity>
		where TEntity : SqlEntity
	{
		private readonly StringBuilder selectBuilder = new StringBuilder();
		private string whereCondition { get; set; }

		public SelectBuilder(List<Expression<Func<TEntity, object>>> propertySelections)
		{
			var table = MetadataResolver.TableName<TEntity>();
			var selections = GetSelectionsFromProperties(propertySelections);

			string selectFrom = $"SELECT {selections} FROM {table} ";
			selectBuilder.Append(selectFrom);
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
					PropertyInfo property = GetProperty(propExpression);

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

		private SelectBuilder(Expression<Func<TEntity, bool>> filterPredicate)
		{
			if (filterPredicate == null)
			{
				throw new ArgumentNullException(nameof(filterPredicate), "Filter predicate expression must be provided.");
			}

			//this.whereCondition = new WhereFilterResolver<TEntity>().FromExpression(filterPredicate);
		}

		// privaet cstr for starting w select

		public static SelectBuilder<TEntity> Select(params Expression<Func<TEntity, object>>[] properties)
		{
			throw new NotImplementedException();
		}

		public static SelectBuilder<TEntity> Where(Expression<Func<TEntity, bool>> filterPredicate)
		{
			return new SelectBuilder<TEntity>(filterPredicate);
		}



		// todo: move to internals?
		public static PropertyInfo GetProperty<T, TProperty>(Expression<Func<T, TProperty>> expression)
			where T : SqlEntity
		{
			MemberExpression memberExpression = null;

			if (expression.Body.NodeType == ExpressionType.Convert)
			{
				memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
			}
			else if (expression.Body.NodeType == ExpressionType.MemberAccess)
			{
				memberExpression = expression.Body as MemberExpression;
			}

			if (memberExpression == null)
			{
				throw new ArgumentException("Not a member access", "expression");
			}

			return memberExpression.Member as PropertyInfo;
		}
	}
}
