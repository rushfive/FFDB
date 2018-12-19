using R5.FFDB.DbProviders.PostgreSql.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql
{
	// Building sql commands uses attributes and reflection,
	// so pre-cache everything for better performance
	public static class EntityInfoMap
	{
		private static Dictionary<Type, string> _tableNames { get; }
		private static Dictionary<Type, List<TableColumnInfo>> _tableColumnInfos { get; }

		private static List<Type> _entityTypes = new List<Type>
		{
			typeof(TeamSql)
		};

		//public static string TableName(Type entityType)
		//{
		//	if (!_tableNames.TryGetValue(entityType, out string name))
		//	{
		//		throw new InvalidOperationException($"Table name doesn't exist for type '{entityType.Name}'.");
		//	}
		//	return name;
		//}

		public static string TableName(Type entityType)
		{
			if (!_tableNames.TryGetValue(entityType, out string name))
			{
				throw new InvalidOperationException($"Table name doesn't exist for type '{entityType.Name}'.");
			}

			return name;
		}

		public static List<TableColumnInfo> ColumnInfos(Type entityType)
		{
			if (!_tableColumnInfos.TryGetValue(entityType, out List<TableColumnInfo> infos))
			{
				throw new InvalidOperationException($"Table column infos don't exist for type '{entityType.Name}'.");
			}
			return infos;
		}

		// initialization

		static EntityInfoMap()
		{
			_tableNames = new Dictionary<Type, string>();
			_tableColumnInfos = new Dictionary<Type, List<TableColumnInfo>>();

			ResolveMapData();
		}

		private static void ResolveMapData()
		{
			_entityTypes.ForEach(t =>
			{
				_tableNames[t] = GetTableName(t);
				_tableColumnInfos[t] = GetColumnInfos(t);
			});
		}

		private static string GetTableName(Type entityType)
		{
			if (entityType.BaseType != typeof(SqlEntity))
			{
				// is this too restrictive?
				throw new ArgumentException($"EntityInfoMap should only deal with types deriving from '{typeof(SqlEntity).Name}'.");
			}

			CustomAttributeData attr = typeof(TeamSql).CustomAttributes
				.SingleOrDefault(a => a.AttributeType == typeof(TableNameAttribute));

			if (attr == null)
			{
				throw new InvalidOperationException($"Entity '{entityType.Name}' is missing its table name.");
			}

			return (string)attr.ConstructorArguments[0].Value;
		}

		private static List<TableColumnInfo> GetColumnInfos(Type entityType)
		{
			var columnInfos = new List<TableColumnInfo>();

			IEnumerable<PropertyInfo> columnProperties = entityType
				.GetProperties()
				.Where(p => p.GetCustomAttributes()
							 .Any(a => a.GetType().BaseType == typeof(EntityColumnAttribute)));

			//var props = teamSql.GetType().GetProperties();
			foreach (PropertyInfo property in columnProperties)
			{
				var info = new TableColumnInfo
				{
					PropertyInfo = property
				};

				foreach (Attribute a in property.GetCustomAttributes())
				{
					switch (a)
					{
						case PrimaryKeyAttribute _:
							info.PrimaryKey = true;
							break;
						case NotNullAttribute _:
							info.NotNull = true;
							break;
						case ColumnAttribute column:
							info.Name = column.Name;
							info.DataType = column.DataType;
							break;
						case ForeignKeyAttribute foreign:
							info.ForeignTableType = foreign.ForeignTableType;
							info.ForeignKeyColumn = foreign.ForeignColumnName;
							break;
					}
				}

				columnInfos.Add(info);
			}

			return columnInfos;
		}
	}

	public class TableColumnInfo
	{
		public string Name { get; set; }
		public PostgresDataType DataType { get; set; }
		public bool PrimaryKey { get; set; }
		public bool NotNull { get; set; }
		public Type ForeignTableType { get; set; }
		public string ForeignKeyColumn { get; set; }
		public PropertyInfo PropertyInfo { get; set; }

		public bool HasForeignKeyConstraint =>
			ForeignTableType != null && !string.IsNullOrWhiteSpace(ForeignKeyColumn);
	}
}
