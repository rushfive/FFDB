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
		{
			string name = EntityInfoMap.TableName(entityType);
			string columns = string.Join(", ", columnsByInfos());

			return $"CREATE TABLE {name} ({columns})";

			// local functions
			IEnumerable<string> columnsByInfos()
			{
				return EntityInfoMap.ColumnInfos(entityType)
					.Select(i =>
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
					});
			}
		}
	}
}
