using Npgsql;
using R5.Internals.Extensions.Reflection;
using R5.Internals.PostgresMapper.Mappers;
using R5.Internals.PostgresMapper.Models;
using R5.Internals.PostgresMapper.SqlBuilders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace R5.Internals.PostgresMapper.QueryCommand
{
	public class UpdateCommand<TEntity> 
		where TEntity : class
	{
		private Func<NpgsqlConnection> _getConnection { get; }
		private ConcatSqlBuilder _sqlBuilder { get; } = new ConcatSqlBuilder();
		private string _whereCondition { get; set; }
		private List<(Expression<Func<TEntity, object>>, object)> _updates { get; } = new List<(Expression<Func<TEntity, object>>, object)>();

		public UpdateCommand(Func<NpgsqlConnection> getConnection)
		{
			_getConnection = getConnection ?? throw new ArgumentNullException(nameof(getConnection));
		}

		public UpdateCommand<TEntity> Where(Expression<Func<TEntity, bool>> conditionExpression)
		{
			_whereCondition = WhereConditionBuilder<TEntity>.FromExpression(conditionExpression);
			return this;
		}

		public UpdateCommand<TEntity> Set(Expression<Func<TEntity, object>> property, object value)
		{
			if (property == null)
			{
				throw new ArgumentNullException(nameof(property), $"Property expression for '{typeof(TEntity).Name}' must be provided.");
			}

			_updates.Add((property, value));
			return this;
		}

		public string GetSqlCommand()
		{
			var tableName = MetadataResolver.TableName<TEntity>();
			_sqlBuilder.Append($"UPDATE {tableName}");

			Dictionary<string, TableColumn> propertyColumnMap = MetadataResolver.PropertyColumnMap<TEntity>();

			var propertySets = new List<string>();
			foreach((Expression<Func<TEntity, object>> property, object value) in _updates)
			{
				PropertyInfo expressionProperty = property.GetProperty();
				
				if (!propertyColumnMap.TryGetValue(expressionProperty.Name, out TableColumn column))
				{
					throw new InvalidOperationException($"Column doesn't exist for property '{expressionProperty.Name}'.");
				}

				var dbValue = ToDbValueStringMapper.Map(value, column.DataType);
				var set = $"{column.Name} = {dbValue}";

				propertySets.Add(set);
			}

			var joinedSets = string.Join(", ", propertySets);
			_sqlBuilder.Append($"SET {joinedSets}");

			if (!string.IsNullOrWhiteSpace(_whereCondition))
			{
				_sqlBuilder.Append($"WHERE {_whereCondition}");
			}

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
