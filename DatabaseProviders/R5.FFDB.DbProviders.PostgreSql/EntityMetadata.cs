using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Attributes;
using R5.FFDB.DbProviders.PostgreSql.Models;
using R5.FFDB.DbProviders.PostgreSql.Models.ColumnInfos;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities.WeekStats;
using R5.Lib.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql
{
	// Caches metadata deriving from attributes and reflection for better performance.
	// Metadata is used in SqlCommandBuilder to dynamically generate SQL queries/commands.
	public static class EntityMetadata
	{
		// Tables are created in the order as defined in this list
		public static List<Type> EntityTypes = new List<Type>
		{
			typeof(TeamSql),
			typeof(PlayerSql),
			typeof(PlayerTeamMapSql),
			typeof(WeekStatsPassSql),
			typeof(WeekStatsRushSql),
			typeof(WeekStatsReceiveSql),
			typeof(WeekStatsMiscSql),
			typeof(WeekStatsKickSql),
			typeof(WeekStatsDstSql),
			typeof(WeekStatsIdpSql),
			typeof(WeekStatsReturnSql),
			typeof(TeamGameStatsSql),
			typeof(UpdateLogSql),
			typeof(WeekGameMatchupSql)
		};

		private static Dictionary<Type, string> _tableNames { get; } = new Dictionary<Type, string>();
		private static Dictionary<Type, List<string>> _compositePrimaryKeys { get; } = new Dictionary<Type, List<string>>();
		private static Dictionary<Type, List<TableColumn>> _tableColumns { get; } = new Dictionary<Type, List<TableColumn>>();
		//private static Dictionary<WeekStatType, PropertyInfo> _weekStatProperty { get; } = new Dictionary<WeekStatType, PropertyInfo>();
		private static Dictionary<WeekStatType, WeekStatColumn> _weekStatColumn { get; } = new Dictionary<WeekStatType, WeekStatColumn>();

		public static string TableName<TEntity>()
			where TEntity : SqlEntity
		{
			return TableName(typeof(TEntity));
		}

		public static string TableName(Type entityType)
		{
			if (!_tableNames.TryGetValue(entityType, out string name))
			{
				throw new InvalidOperationException($"Table name doesn't exist for type '{entityType.Name}'.");
			}

			return name;
		}

		public static bool TryGetCompositePrimaryKeys(Type entityType, out List<string> keys)
		{
			return _compositePrimaryKeys.TryGetValue(entityType, out keys);
		}

		public static List<TableColumn> Columns(Type entityType)
		{
			if (!_tableColumns.TryGetValue(entityType, out List<TableColumn> columns))
			{
				throw new InvalidOperationException($"Table columns don't exist for type '{entityType.Name}'.");
			}
			return columns;
		}

		public static WeekStatColumn GetWeekStatColumnByType(WeekStatType type)
		{
			if (!_weekStatColumn.TryGetValue(type, out WeekStatColumn column))
			{
				throw new InvalidOperationException($"Failed to find week stat column mapped to week stat type '{type}'.");
			}
			return column;
		}

		//public static PropertyInfo GetPropertyByStat(WeekStatType type)
		//{
		//	if (!_weekStatProperty.TryGetValue(type, out PropertyInfo info))
		//	{
		//		throw new InvalidOperationException($"Failed to find property info mapped to week stat type '{type}'.");
		//	}
		//	return info;
		//}

		// initialization
		static EntityMetadata()
		{
			CacheMetadata();
		}

		private static void CacheMetadata()
		{
			EntityTypes.ForEach((Action<Type>)(t =>
			{
				_tableNames[t] = GetTableName(t);
				_tableColumns[t] = EntityMetadata.GetColumns((Type)t);

				List<string> compositeKeys = GetCompositePrimaryKeys(t);
				if (compositeKeys != null)
				{
					_compositePrimaryKeys[t] = compositeKeys;
				}
			}));
		}

		private static string GetTableName(Type entityType)
		{
			TableNameAttribute attr = entityType.GetCustomAttributeOrNull<TableNameAttribute>();

			if (attr == null)
			{
				throw new InvalidOperationException($"Entity '{entityType.Name}' is missing its table name.");
			}

			return attr.Name;
		}

		private static List<string> GetCompositePrimaryKeys(Type entityType)
		{
			CompositePrimaryKeysAttribute attr = entityType.GetCustomAttributeOrNull<CompositePrimaryKeysAttribute>();

			return attr?.ColumnNames;
		}

		private static List<TableColumn> GetColumns(Type entityType)
		{
			var result = new List<TableColumn>();

			List<PropertyColumn> propertyColumns = GetPropertyColumns(entityType);
			result.AddRange(propertyColumns);

			List<WeekStatColumn> weekStatColumns = GetWeekStatColumns(entityType);
			foreach (WeekStatColumn c in weekStatColumns)
			{
				result.Add(c);
				_weekStatColumn[c.StatType] = c;
				//_weekStatProperty[c.StatType] = c.Property;
			}

			return result;
		}

		private static List<PropertyColumn> GetPropertyColumns(Type entityType)
		{
			return entityType
				.GetPropertiesContainingAttribute<ColumnAttribute>()
				.Select(PropertyColumn.FromProperty)
				.ToList();
		}

		private static List<WeekStatColumn> GetWeekStatColumns(Type entityType)
		{
			return entityType
				.GetPropertiesContainingAttribute<WeekStatColumnAttribute>()
				.Select(WeekStatColumn.FromProperty)
				.ToList();
		}
	}
}
