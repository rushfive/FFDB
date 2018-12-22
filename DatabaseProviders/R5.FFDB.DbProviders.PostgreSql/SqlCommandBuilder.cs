using R5.FFDB.DbProviders.PostgreSql.Models;
using R5.FFDB.DbProviders.PostgreSql.Models.ColumnInfos;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
using System;
using System.Collections.Generic;
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
				IEnumerable<string> columnsSqlList = EntityInfoMap.ColumnInfos(entityType)
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

				string nameSql = EntityInfoMap.TableName(entityType);
				string columnsSql = string.Join(", ", columnsSqlList);

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
					sql += $"REFERENCES {EntityInfoMap.TableName(info.ForeignTableType)}({info.ForeignKeyColumn})";
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
				string tableName = EntityInfoMap.TableName(typeof(T));

				List<ColumnInfo> columnInfos = EntityInfoMap.ColumnInfos(typeof(T));
				string columnsSql = string.Join(", ", columnInfos.Select(i => i.Name));

				string sql = $"INSERT INTO {tableName} ({columnsSql}) VALUES ";

				IEnumerable<string> entityValues = entities.Select(e => EntityValues(columnInfos, e));
				sql += string.Join(", ", entityValues);

				return sql + ";";
			}

			public static string EntityValues<T>(List<ColumnInfo> columnInfos, T entity)
				where T : SqlEntity
			{
				var values = new List<string>();

				foreach (ColumnInfo info in columnInfos)
				{
					object value = info.Property.GetValue(entity);

					// column based validations for value
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

			public static string ValueStringByDataType(PostgresDataType dataType, object value)
			{
				if (value == null)
				{
					return "null";
				}

				switch (dataType)
				{
					case PostgresDataType.UUID:
					case PostgresDataType.TEXT:
						return $"'{value}'";
					case PostgresDataType.INT:
					case PostgresDataType.FLOAT8:
						return value.ToString();
					case PostgresDataType.DATE:
						return $"'{((DateTimeOffset)value).ToUniversalTime().ToString("yyyy-MM-dd")}'";
					case PostgresDataType.TIMESTAMPTZ:
						throw new NotImplementedException();
					default:
						throw new ArgumentOutOfRangeException($"'{dataType}' is not a valid postgres data type.");
				}
			}
		}
	}
}
