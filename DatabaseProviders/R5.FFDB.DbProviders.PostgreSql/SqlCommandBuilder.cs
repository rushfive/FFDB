using R5.FFDB.DbProviders.PostgreSql.Models;
using R5.FFDB.DbProviders.PostgreSql.Models.ColumnInfos;
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

				return $"CREATE TABLE {nameSql} ({columnsSql})";
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

		
	}
}
