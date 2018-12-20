using R5.FFDB.DbProviders.PostgreSql.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql
{
	public static class SqlEntityCommandBuilder
	{
		public static string CreateTable(Type entityType) 
			//List<PropertyColumnInfo> additionalColumns = null)
		{
			List<ColumnInfo> columns = EntityInfoMap.ColumnInfos(entityType);

			//if (additionalColumns?.Any() == true)
			//{
			//	columns.AddRange(additionalColumns);
			//}

			string nameSql = EntityInfoMap.TableName(entityType);
			string columnsSql = string.Join(", ", columnsByInfos());

			return $"CREATE TABLE {nameSql} ({columnsSql})";

			// local functions
			IEnumerable<string> columnsByInfos()
			{
				return columns.Select(i =>
				{
					if (i is PropertyColumnInfo pi)
					{
						return columnByPropertyInfo(pi);
					}
					throw new NotImplementedException("handle week stat col type.");
				});
			}

			string columnByPropertyInfo(PropertyColumnInfo i)
			{
				string sql = $"{i.Name} {i.DataType} ";

				if (i.PrimaryKey)
				{
					sql += "PRIMARY KEY ";
				}
				else if (i.NotNull)
				{
					sql += "NOT NULL ";
				}

				if (i.HasForeignKeyConstraint)
				{
					sql += $"REFERENCES {EntityInfoMap.TableName(i.ForeignTableType)}({i.ForeignKeyColumn})";
				}

				return sql.TrimEnd();
			}
		}
	}
}
