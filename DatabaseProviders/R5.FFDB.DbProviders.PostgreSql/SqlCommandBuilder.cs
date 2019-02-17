using R5.FFDB.DbProviders.PostgreSql.Models;
using R5.FFDB.DbProviders.PostgreSql.Models.ColumnInfos;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql
{
	public static class SqlCommandBuilder
	{
		public static class Table
		{
			public static string Create(Type entityType)
			{
				IEnumerable<string> columnsSqlList = EntityMetadata.ColumnInfos(entityType)
					.Select(c =>
					{
						switch (c)
						{
							case PropertyColumnInfo property:
								return ColumnByPropertyColumn(property);
							case WeekStatColumnInfo weekStat:
								return ColumnByWeekStatColumn(weekStat);
							default:
								throw new ArgumentOutOfRangeException($"Invalid column info type '{c.GetType().Name}'.");
						}
					});

				string nameSql = EntityMetadata.TableName(entityType);
				string columnsSql = string.Join(", ", columnsSqlList);

				if (EntityMetadata.TryGetCompositePrimaryKeys(entityType, out List<string> compositeKeys))
				{
					columnsSql += $", PRIMARY KEY({string.Join(", ", compositeKeys)})";
				}

				return $"CREATE TABLE {nameSql} ({columnsSql});";
			}

			private static string ColumnByPropertyColumn(PropertyColumnInfo info)
			{
				string sql = $"{info.Name} {info.DataType} ";

				if (info.PrimaryKey)
				{
					sql += "PRIMARY KEY ";
				}
				else if (info.NotNull)
				{
					sql += "NOT NULL ";
				}

				if (info.HasForeignKeyConstraint)
				{
					sql += $"REFERENCES {EntityMetadata.TableName(info.ForeignTableType)}({info.ForeignKeyColumn})";
				}

				return sql.TrimEnd();
			}

			private static string ColumnByWeekStatColumn(WeekStatColumnInfo info)
			{
				return $"{info.Name} {info.DataType}";
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

				List<ColumnInfo> columnInfos = EntityMetadata.ColumnInfos(typeof(T));
				string columnsSql = string.Join(", ", columnInfos.Select(i => i.Name));

				string sql = $"INSERT INTO {tableName} ({columnsSql}) VALUES ";

				IEnumerable<string> entityValues = entities.Select(e => EntityValues(columnInfos, e, excludeKeys: false));
				sql += string.Join(", ", entityValues);

				return sql + ";";
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

				List<ColumnInfo> columnInfos = EntityMetadata.ColumnInfos(typeof(T));

				// dont update columns that are primary keys, or used as composite primary keys
				Func<ColumnInfo, bool> usedAsKey = info =>
				{
					var propertyInfo = info as PropertyColumnInfo;
					if (propertyInfo == null)
					{
						return false;
					}

					return propertyInfo.PrimaryKey || compositePrimaryKeys.Contains(propertyInfo.Name);
				};

				columnInfos = columnInfos.Where(i => !usedAsKey(i)).ToList();

				string columnsSql = string.Join(", ", columnInfos.Select(i => i.Name));
				string values = EntityValues(columnInfos, entity, excludeKeys: true);

				return $"UPDATE {tableName} SET ({columnsSql}) = {values} WHERE {entity.PrimaryKeyMatchCondition()};";
			}

			private static string EntityValues<T>(List<ColumnInfo> columnInfos, T entity, bool excludeKeys)
				where T : SqlEntity
			{
				var values = new List<string>();

				foreach (ColumnInfo info in columnInfos)
				{
					object value = info.Property.GetValue(entity);

					// column based validations for value
					// REFACTOR into better validating strategy
					switch (info)
					{
						case PropertyColumnInfo property:
							if (value == null && property.NotNull)
							{
								throw new InvalidOperationException($"Column '{info.Name}' on type '{entity.GetType().Name}' requires a value.");
							}
							if (property.DataType == PostgresDataType.UUID)
							{
								bool isGuidEmpty = (value.GetType() == typeof(Guid) && (Guid)value == Guid.Empty)
									|| (value.GetType() == typeof(Guid?) && ((Guid?)value).HasValue && ((Guid?)value).Value == Guid.Empty);
								if (isGuidEmpty)
								{
									throw new InvalidOperationException($"Column '{info.Name}' on type '{entity.GetType().Name}' requires a valid Guid value.");
								}
							}
							break;
						case WeekStatColumnInfo weekStat:
							break;
						default:
							throw new ArgumentOutOfRangeException($"'{info.GetType().Name}' is not a valid '{nameof(ColumnInfo)}' type.");
					}

					string valueString = ValueStringByDataType(info.DataType, value);
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
