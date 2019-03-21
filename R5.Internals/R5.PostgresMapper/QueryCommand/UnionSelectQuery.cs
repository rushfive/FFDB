using Npgsql;
using R5.Internals.Extensions.Reflection;
using R5.Internals.PostgresMapper.Mappers;
using R5.Internals.PostgresMapper.Models;
using R5.Internals.PostgresMapper.SqlBuilders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace R5.Internals.PostgresMapper.QueryCommand
{
	// todo internal
	// for now, allows selecting of a single column (property Types must match). The property type
	// also shouldnt be a complex type (eg custom class).
	// in the future, allow for selecting of more cols using aliases, and by allowing a mapper
	// to be provided for this task
	public class UnionSelectQuery<TResult>
	{
		private Func<NpgsqlConnection> _getConnection { get; }
		private ConcatSqlBuilder _sqlBuilder { get; } = new ConcatSqlBuilder();
		private List<string> _selects { get; } = new List<string>();

		private bool _metadataResolved => _propertyType != null && _dataType.HasValue;
		private Type _propertyType { get; set; }
		private PostgresDataType? _dataType { get; set; }

		public UnionSelectQuery(
			Func<NpgsqlConnection> getConnection)
		{
			_getConnection = getConnection ?? throw new ArgumentNullException(nameof(getConnection));

			if (!SupportedTypes.IsSupported<TResult>())
			{
				throw new ArgumentException($"The type '{typeof(TResult).Name}' cannot be used as the result of a Union query.", nameof(TResult));
			}
		}

		public UnionSelectQuery<TResult> From<TEntity>(Action<SelectFromBuilder<TEntity>> configure)
		{
			if (configure == null)
			{
				throw new ArgumentNullException(nameof(configure), "Select builder configure action must be provided.");
			}

			var builder = new SelectFromBuilder<TEntity>();
			configure.Invoke(builder);

			_selects.Add(builder.BuildQuery());

			if (!_metadataResolved)
			{
				(Type propertyType, PostgresDataType dataType) = builder.GetMetadata();
				_propertyType = propertyType;
				_dataType = dataType;
			}

			return this;
		}

		public string GetSqlCommand()
		{
			if (!_selects.Any())
			{
				throw new InvalidOperationException("At least one selection must be configured.");
			}

			var enclosedSelects = _selects.Select(s => $"({s})");
			var unioned = string.Join(" UNION ", enclosedSelects);

			return $"{unioned};";
		}

		public Task<List<TResult>> ExecuteAsync()
		{
			var sqlCommand = GetSqlCommand();
#if DEBUG
			DebugUtil.OutputSqlCommand(sqlCommand);
#endif
			NpgsqlConnection connection = _getConnection();
			Func<NpgsqlDataReader, List<TResult>> mapper = UnionResultMapperFactory.Create<TResult>(_propertyType, _dataType.Value);

			return connection.ExecuteReaderAsync(sqlCommand, mapper);
		}

		public class SelectFromBuilder<TEntity>
		{
			private ConcatSqlBuilder _sqlBuilder { get; } = new ConcatSqlBuilder();
			private Expression<Func<TEntity, TResult>> _selection { get; set; }
			private Expression<Func<TEntity, bool>> _where { get; set; }
			
			public SelectFromBuilder<TEntity> Property(Expression<Func<TEntity, TResult>> selection)
			{
				if (selection == null)
				{
					throw new ArgumentNullException(nameof(selection), "Property selection expression must be provided.");
				}

				// validate expression
				PropertySelectionResolver.ValidatePropertyExpression(selection);

				_selection = selection;
				return this;
			}

			public SelectFromBuilder<TEntity> Where(Expression<Func<TEntity, bool>> where)
			{
				if (where == null)
				{
					throw new ArgumentNullException(nameof(where), "Condition expression must be provided.");
				}
				
				_where = where;
				return this;
			}

			public string BuildQuery()
			{
				var table = MetadataResolver.TableName<TEntity>();
				var selection = PropertySelectionResolver.GetSelectionFromProperty(_selection);

				string selectFrom = $"SELECT {selection} FROM {table}";
				_sqlBuilder.Append(selectFrom);

				if (_where != null)
				{
					var whereCondition = WhereConditionBuilder<TEntity>.FromExpression(_where);
					_sqlBuilder.Append($"WHERE {whereCondition}");
				}

				return _sqlBuilder.GetResult(omitTerminatingSemiColon: true);
			}

			public (Type propertyType, PostgresDataType dataType) GetMetadata()
			{
				if (_selection == null)
				{
					throw new InvalidOperationException("Selection must be configured to get metadata.");
				}

				if (!PropertySelectionResolver.TryGetColumnFromExpression(_selection, out TableColumn column))
				{
					throw new InvalidOperationException("Failed to resolve metadata from selection expression.");
				}

				return (column.PropertyType, column.DataType);
			}
		}
	}

	public static class UnionResultMapperFactory
	{
		public static Func<NpgsqlDataReader, List<TResult>> Create<TResult>(Type propertyType, PostgresDataType dataType)
		{
			return reader =>
			{
				var result = new List<object>();

				while (reader.Read())
				{
					Debug.Assert(reader.FieldCount == 1, "Union queries only support selecting a single column.");
					
					object value = reader.GetValue(0);
					object resolvedValue = DbValueToObjectMapper.Map(value, propertyType, dataType);

					result.Add(resolvedValue);
				}

				return result.Cast<TResult>().ToList();
			};
		}
	}


}
