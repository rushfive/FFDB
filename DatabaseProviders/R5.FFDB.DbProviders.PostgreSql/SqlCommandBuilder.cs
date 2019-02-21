using R5.FFDB.DbProviders.PostgreSql.Models;
using R5.FFDB.DbProviders.PostgreSql.Models.ColumnInfos;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql
{
	public static class SqlCommandBuilder
	{
		public static class Table
		{
			public static string Create(Type entityType)
			{
				IEnumerable<string> columns = EntityMetadata.Columns(entityType)
					.Select(c => c.GetSqlColumnDefinition());

				string nameSql = EntityMetadata.TableName(entityType);
				string columnsSql = string.Join(", ", columns);

				if (EntityMetadata.TryGetCompositePrimaryKeys(entityType, out List<string> compositeKeys))
				{
					columnsSql += $", PRIMARY KEY({string.Join(", ", compositeKeys)})";
				}

				return $"CREATE TABLE {nameSql} ({columnsSql});";
			}
		}

		public static class Rows
		{
			public static string Insert<T>(T entity)
				where T : SqlEntity
			{
				return InsertMany(new List<T> { entity });
			}

			public static string InsertMany<T>(IEnumerable<T> entities)
				where T : SqlEntity
			{
				string tableName = EntityMetadata.TableName(typeof(T));

				List<TableColumn> columns = EntityMetadata.Columns(typeof(T));
				string columnsSql = string.Join(", ", columns.Select(i => i.Name));

				string sql = $"INSERT INTO {tableName} ({columnsSql}) VALUES ";

				IEnumerable<string> entityValues = entities.Select(e => EntityValues(columns, e, excludeKeys: false));
				sql += string.Join(", ", entityValues);

				return sql + ";";
			}

			public static string UpdateNEW<T>(string whereCondition, 
				params (Expression<Func<T, object>> propertyExpression, object value)[] updates)
				where T : SqlEntity
			{
				if (updates == null || !updates.Any())
				{
					throw new ArgumentNullException(nameof(updates), "Property updates list must be provided to perform update.");
				}

				var tableName = EntityMetadata.TableName<T>();

				List<TableColumn> columns = EntityMetadata.Columns(typeof(T));
				Dictionary<string, TableColumn> propertyColumnMap = columns.ToDictionary(c => c.GetPropertyName(), c => c);

				var setClauses = new List<string>();
				foreach(var update in updates)
				{
					PropertyInfo updateProperty = GetPropertyInfoFromExpression(update.propertyExpression);
					
					if (!propertyColumnMap.TryGetValue(updateProperty.Name, out TableColumn col))
					{
						throw new InvalidOperationException($"Failed to resolve property '{updateProperty.Name}' to a table column for '{tableName}'.");
					}

					setClauses.Add($"{col.Name} = {ValueStringByDataType(col.DataType, update.value)}");
				}


				string sql = $"UPDATE {tableName} SET {string.Join(", ", setClauses)}";
				if (!string.IsNullOrWhiteSpace(whereCondition))
				{
					//sql += $" {whereCondition}";
					sql = sql + " " + whereCondition;
				}
				sql += ";";

				return sql;
			}

			

			private static PropertyInfo GetPropertyInfoFromExpression<T>(Expression<Func<T, object>> expression)
				where T : SqlEntity
			{
				var memberExpression = expression.Body as MemberExpression;
				if (memberExpression == null)
				{
					throw new ArgumentException($"Failed to read the property expression body as a '{nameof(MemberExpression)}' type.");
				}

				var propertyInfo = memberExpression.Member as PropertyInfo;
				if (propertyInfo == null)
				{
					throw new ArgumentException($"Failed to read the member expression as a '{nameof(PropertyInfo)}' type.");
				}

				return propertyInfo;
			}

			public static string Update<T>(T entity)
				where T : SqlEntity
			{
				string tableName = EntityMetadata.TableName(typeof(T));

				var compositePrimaryKeys = new HashSet<string>();
				if (EntityMetadata.TryGetCompositePrimaryKeys(typeof(T), out List<string> compositeKeys))
				{
					compositePrimaryKeys = compositeKeys.ToHashSet();
				}

				List<TableColumn> columns = EntityMetadata.Columns(typeof(T));

				// dont update columns that are primary keys, or used as composite primary keys
				Func<TableColumn, bool> usedAsKey = info =>
				{
					var propertyInfo = info as PropertyColumn;
					if (propertyInfo == null)
					{
						return false;
					}

					return propertyInfo.PrimaryKey || compositePrimaryKeys.Contains(propertyInfo.Name);
				};

				columns = columns.Where(i => !usedAsKey(i)).ToList();

				string columnsSql = string.Join(", ", columns.Select(i => i.Name));
				string values = EntityValues(columns, entity, excludeKeys: true);

				return $"UPDATE {tableName} SET ({columnsSql}) = {values} WHERE {entity.PrimaryKeyMatchCondition()};";
			}

			private static string EntityValues<T>(List<TableColumn> columns, T entity, bool excludeKeys)
				where T : SqlEntity
			{
				var values = new List<string>();

				foreach (TableColumn col in columns)
				{
					object value = col.GetValue(entity);

					// column based validations for value
					// REFACTOR into better validating strategy
					switch (col)
					{
						case PropertyColumn property:
							if (value == null && property.NotNull)
							{
								throw new InvalidOperationException($"Column '{col.Name}' on type '{entity.GetType().Name}' requires a value.");
							}
							if (property.DataType == PostgresDataType.UUID)
							{
								bool isGuidEmpty = (value.GetType() == typeof(Guid) && (Guid)value == Guid.Empty)
									|| (value.GetType() == typeof(Guid?) && ((Guid?)value).HasValue && ((Guid?)value).Value == Guid.Empty);
								if (isGuidEmpty)
								{
									throw new InvalidOperationException($"Column '{col.Name}' on type '{entity.GetType().Name}' requires a valid Guid value.");
								}
							}
							break;
						case WeekStatColumn weekStat:
							break;
						default:
							throw new ArgumentOutOfRangeException($"'{col.GetType().Name}' is not a valid '{nameof(TableColumn)}' type.");
					}

					string valueString = ValueStringByDataType(col.DataType, value);
					values.Add(valueString);
				}

				return $"({string.Join(", ", values)})";
			}

			private static string ValueStringByDataType(PostgresDataType dataType, object value)
			{
				if (value == null)
				{
					return "null";
				}

				switch (dataType)
				{
					case PostgresDataType.UUID:
						return $"'{value}'";
					case PostgresDataType.TEXT:
						return $"'{value.ToString().Replace("'", "''")}'";
					case PostgresDataType.INT:
					case PostgresDataType.FLOAT8:
						return value.ToString();
					case PostgresDataType.DATE:
						return $"'{((DateTimeOffset)value).ToUniversalTime().ToString("yyyy-MM-dd")}'";
					case PostgresDataType.TIMESTAMPTZ:
						return $"'{((DateTimeOffset)value).ToUniversalTime().ToString("o", CultureInfo.InvariantCulture)}'";
					case PostgresDataType.BOOLEAN:
						{
							bool boolVal = (bool)value;
							return boolVal ? "true" : "false";
						}
					default:
						throw new ArgumentOutOfRangeException($"'{dataType}' is not a valid postgres data type.");
				}
			}

			public static string DeleteAll(Type entityType)
			{
				// todo: validate entityType is SqlEntity
				string tableName = EntityMetadata.TableName(entityType);
				return $"DELETE FROM {tableName};";
			}

			public static string Delete<T>(T entity)
				where T : SqlEntity
			{
				string tableName = EntityMetadata.TableName(typeof(T));
				return $"DELETE FROM {tableName} WHERE {entity.PrimaryKeyMatchCondition()};";
			}
		}
	}
}
