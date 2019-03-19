using R5.Internals.Extensions.Reflection;
using R5.Internals.PostgresMapper.Mappers;
using R5.Internals.PostgresMapper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace R5.Internals.PostgresMapper.SqlBuilders
{
	public class WhereConditionBuilder<TEntity> : ExpressionVisitor
	{
		private static readonly Dictionary<ExpressionType, string> _operatorMap;

		private readonly StringBuilder _whereFilterBuilder = new StringBuilder();
		private readonly Dictionary<string, TableColumn> _propertyColumns;
		private readonly Stack<TableColumn> _columnStack = new Stack<TableColumn>();

		static WhereConditionBuilder()
		{
			// static constructor is called for each closed class type, init operators once
			if (_operatorMap == null)
			{
				_operatorMap = new Dictionary<ExpressionType, string>
				{
					[ExpressionType.Equal] = "=",
					[ExpressionType.GreaterThan] = ">",
					[ExpressionType.LessThan] = "<",
					[ExpressionType.GreaterThanOrEqual] = ">=",
					[ExpressionType.LessThanOrEqual] = "<=",
					[ExpressionType.NotEqual] = "!=",
					[ExpressionType.AndAlso] = "AND",
					[ExpressionType.OrElse] = "OR",
					[ExpressionType.Not] = "NOT",
				};
			}
		}

		private WhereConditionBuilder()
		{
			_propertyColumns = MetadataResolver.PropertyColumnMap<TEntity>();
		}

		private static bool IsSupportedOperator(ExpressionType type)
			=> _operatorMap.ContainsKey(type);

		public static string FromExpression(LambdaExpression predicateExpression)
		{
			var resolver = new WhereConditionBuilder<TEntity>();

			resolver.Visit(predicateExpression.Body);

			return resolver.GetResult();
		}

		private string GetResult()
		{
			var result = _whereFilterBuilder.ToString();

			_whereFilterBuilder.Clear();

			return result;
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			if (node.NodeType == ExpressionType.Not && node.Operand is MemberExpression member)
			{
				string memberName = member.Member.Name;

				if (!_propertyColumns.TryGetValue(memberName, out TableColumn column))
				{
					throw new InvalidOperationException($"Failed to find column matching property '{memberName}'.");
				}

				_whereFilterBuilder.Append($"NOT {column.Name}");
			}
			else if (node.Operand.NodeType == ExpressionType.Convert || node.Operand.NodeType == ExpressionType.Constant)
			{
				Visit(node.Operand);
			}
			else if (!IsSupportedOperator(node.NodeType))
			{
				throw new NotSupportedException($"UnaryExpressions of type '{node.NodeType}' are not supported for building where-conditions.");
			}

			return node;
		}

		// handles operators like equal, and, or, gt, etc
		protected override Expression VisitBinary(BinaryExpression node)
		{
			if (!IsSupportedOperator(node.NodeType))
			{
				throw new NotSupportedException($"BinaryExpressions of type '{node.NodeType}' are not supported for building where-conditions.");
			}

			_whereFilterBuilder.Append("(");

			Visit(node.Left);

			_whereFilterBuilder.Append($" {_operatorMap[node.NodeType]} ");
			
			Visit(node.Right);

			_whereFilterBuilder.Append(")");

			return node;
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			TableColumn column = _columnStack.Pop();

			var dbStringValue = ToDbValueStringMapper.Map(node.Value, column.DataType);
			
			_whereFilterBuilder.Append(dbStringValue);

			return node;
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			//switch (node.Member)
			//{
			//	case FieldInfo fieldInfo:
			//		{
			//			if (node.Expression is ConstantExpression constExpr)
			//			{
			//				var value = constExpr.Value;

			//				var fieldValue = fieldInfo.GetValue(value);
			//			}

			//			if (node.Expression.NodeType == ExpressionType.Constant)
			//			{
							
			//			}
			//		}
			//		break;
			//	case PropertyInfo propertyInfo:
			//		break;
			//}

			
			if (node.Member is FieldInfo fieldInfo
				&& node.Expression is ConstantExpression constExpr)
			{
				var value = constExpr.Value;
				var fieldValue = fieldInfo.GetValue(value);
				_whereFilterBuilder.Append(fieldValue);
			}
			else if (node.Member is PropertyInfo propertyInfo
				&& node.Expression is MemberExpression memberExpr
					&& memberExpr.Member is FieldInfo memberFieldInfo
					&& memberExpr.Expression is ConstantExpression memberConstExpr)
			{
				var value = memberConstExpr.Value;
				var memberValue = memberFieldInfo.GetValue(value);
			}
			//else if (node.Member is PropertyInfo propertyInfo)
			//{
			//	if (node.Expression is MemberExpression memberExpr
			//		&& memberExpr.Member is FieldInfo memberFieldInfo
			//		&& memberExpr.Expression is ConstantExpression memberConstExpr)
			//	{
			//		var value = memberConstExpr.Value;
			//		var memberValue = memberFieldInfo.GetValue(value);
			//	}

			//	var nodeExpr = node.Expression;
			//}
			else
			{
				if (!_propertyColumns.TryGetValue(node.Member.Name, out TableColumn column))
				{
					throw new InvalidOperationException($"Failed to find column associated to property "
						+ $"'{node.Member.Name}' on entity type '{typeof(TEntity).Name}'.");
				}

				_columnStack.Push(column);

				_whereFilterBuilder.Append(column.Name);
			}

			//
			//if (!_propertyColumns.TryGetValue(node.Member.Name, out TableColumn column))
			//{
			//	throw new InvalidOperationException($"Failed to find column associated to property "
			//		+ $"'{node.Member.Name}' on entity type '{typeof(TEntity).Name}'.");
			//}

			//_columnStack.Push(column);
			
			//_whereFilterBuilder.Append(column.Name);

			return node;
		}
	}
}
