using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace R5.Internals.Extensions.Reflection
{
	public static class ExpressionExtensions
	{
		public static PropertyInfo GetProperty<T, TProperty>(this Expression<Func<T, TProperty>> expression)
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
