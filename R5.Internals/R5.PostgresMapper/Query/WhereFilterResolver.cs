using R5.Internals.Extensions.Reflection;
using R5.Internals.PostgresMapper.Mappers;
using R5.Internals.PostgresMapper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace R5.Internals.PostgresMapper.Query
{
	public class WhereFilterResolver<TEntity> : ExpressionVisitor
			where TEntity : SqlEntity
	{
		private static readonly Dictionary<ExpressionType, string> _operatorMap;
		private static readonly Dictionary<Type, Dictionary<string, TableColumn>> _entityPropertyColumns = new Dictionary<Type, Dictionary<string, TableColumn>>();

		private readonly StringBuilder _whereFilterBuilder = new StringBuilder();
		private readonly Dictionary<string, TableColumn> _propertyColumns;
		private readonly Stack<TableColumn> _columnStack = new Stack<TableColumn>();

		static WhereFilterResolver()
		{
			// static constructor is called for each closed class type, 
			// init operators once
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
					[ExpressionType.OrElse] = "OR"
				};
			}

			Type entityType = typeof(TEntity);

			if (!_entityPropertyColumns.ContainsKey(entityType))
			{
				_entityPropertyColumns[entityType] = new Dictionary<string, TableColumn>();

				List<TableColumn> columns = MetadataResolver.TableColumns(entityType);
				foreach(var col in columns)
				{
					_entityPropertyColumns[entityType][col.GetPropertyName()] = col;
				}
			}
		}

		public WhereFilterResolver()
		{
			_propertyColumns = _entityPropertyColumns[typeof(TEntity)];
		}

		private static bool IsSupportedOperator(ExpressionType type)
			=> _operatorMap.ContainsKey(type);

		public string FromExpression(LambdaExpression predicateExpression)
		{
			Visit(predicateExpression.Body);

			var result = _whereFilterBuilder.ToString();

			_whereFilterBuilder.Clear();

			return result;
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			if (node.Operand.NodeType == ExpressionType.Convert || node.Operand.NodeType == ExpressionType.Constant)
			{
				Visit(node.Operand);
			}
			else if (!IsSupportedOperator(node.NodeType))
			{
				throw new NotSupportedException("Unary expressions not supported for building where filters.");
			}

			return node;
		}

		// handles operators like equal, and, or, gt, etc
		protected override Expression VisitBinary(BinaryExpression node)
		{
			if (!IsSupportedOperator(node.NodeType))
			{
				throw new NotSupportedException("Unary expressions not supported for building where filters.");
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
			if (!_propertyColumns.TryGetValue(node.Member.Name, out TableColumn column))
			{
				throw new InvalidOperationException($"Failed to find column associated to property "
					+ $"'{node.Member.Name}' on entity type '{typeof(TEntity).Name}'.");
			}

			_columnStack.Push(column);
			
			_whereFilterBuilder.Append(column.Name);

			return node;
		}
	}
}
