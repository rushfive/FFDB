using R5.Internals.PostgresMapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace R5.Internals.PostgresMapper.Builders
{
	public class QueryBuilder
	{
		public static void Test2<TEntity, TProperty>(
			Expression<Func<TEntity, TProperty>> propertyExpression,
			Expression<Func<TProperty, bool>> propertyPredicate)
			where TEntity : SqlEntity
		{
			// get type of TProperty
			Expression propertyBody = propertyExpression.Body;
			MemberExpression memberExpr = (MemberExpression)propertyBody;
			PropertyInfo property = GetProperty(propertyExpression);

			//predicateBody.GetType().Name
			//"MethodBinaryExpression"
			Expression predicateBody = propertyPredicate.Body;

			Type type = predicateBody.GetType();

			string name1 = predicateBody.GetType().Name;
			string name2 = predicateBody.Type.Name;

			// for this to work, we need to enforce a very specific
			// expression structure:
			// its a BINARY expression
			// where the left is a ___ representing the entity
			// the right must be a bool result type, but can be
			// a chain of expressions, where all of them
			// reference the entity as a comparison point


			var binaryExpression = predicateBody as BinaryExpression;
			if (binaryExpression != null)
			{
				Expression right = binaryExpression.Right;

				var rightConstant = right as ConstantExpression;
				if (rightConstant != null)
				{
					object rightValue = rightConstant.Value;
					Type rightValueType = rightConstant.Type;

					string tttt = "Tttt";
				}


				string rightTypeName = right.GetType().Name;
				string rightTypeName2 = right.Type.Name;

				



				
			}

			

			//if (predicateBody is MethodBinaryExpression)

			string tetsets = "Tawetaw";
			//	var something = new List<int>().Where()
		}





		public static void Test<TEntity>(Expression<Func<TEntity, bool>> predicate)
			where TEntity : SqlEntity
		{
			MemberExpression memberExpr = (MemberExpression)predicate.Body;

			Expression strExpr = memberExpr.Expression;






			// returns the proeprty 'NullableDouble'
			//PropertyInfo entityProperty = GetProperty<TestEntity>(e => e.NullableDouble);




			string t = "Test";
		}


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
