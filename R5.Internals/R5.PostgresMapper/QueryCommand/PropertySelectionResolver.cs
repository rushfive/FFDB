using R5.Internals.Extensions.Reflection;
using R5.Internals.PostgresMapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace R5.Internals.PostgresMapper.QueryCommand
{
	// todo internal
	public static class PropertySelectionResolver
	{
		// expected to be a lambda, with the body being a simple MemberAccess
		// todo: any way to get compile time checks on these??
		public static void ValidatePropertyExpression<TEntity, TProp>(Expression<Func<TEntity, TProp>> expression)
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

			// might have to move the 2nd part, checking for base type outside of methpod
			// or make it optional. added it to fix and test union selection querys
			if (member.Member.DeclaringType != entityType && member.Member.DeclaringType != entityType.BaseType)
			{
				throw new ArgumentException($"Expression must reference a property from class '{entityType.Name}");
			}
		}

		public static string GetSelectionsFromProperties<TEntity>(List<Expression<Func<TEntity, object>>> propertySelections)
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
					if (!TryGetColumnFromExpression(propExpression, out TableColumn column))
					{
						throw new ArgumentException($"Failed to find matching table column.", nameof(propExpression));
					}

					columns.Add(column.Name);
				}

				selections = string.Join(", ", columns);
			}

			return selections;
		}

		public static string GetSelectionFromProperty<TEntity, TProp>(Expression<Func<TEntity, TProp>> property)
		{
			if (!TryGetColumnFromExpression(property, out TableColumn column))
			{
				throw new ArgumentException($"Property '{property.Name}' derived from expression is invalid: failed to find matching table column.");
			}

			return column.Name;
		}

		public static bool TryGetColumnFromExpression<TEntity, TProp>(Expression<Func<TEntity, TProp>> expression, out TableColumn column)
		{
			column = null;

			Dictionary<string, TableColumn> propertyColumnMap = MetadataResolver.PropertyColumnMap<TEntity>();
			PropertyInfo property = expression.GetProperty();

			if (!propertyColumnMap.TryGetValue(property.Name, out column))
			{
				return false;
			}

			return true;
		}
	}
}
