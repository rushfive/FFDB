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
		private static Dictionary<Type, List<ColumnInfo>> _tableColumnInfos { get; } = new Dictionary<Type, List<ColumnInfo>>();
		private static Dictionary<WeekStatType, PropertyInfo> _weekStatProperty { get; } = new Dictionary<WeekStatType, PropertyInfo>();

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

		public static List<ColumnInfo> ColumnInfos(Type entityType)
		{
			if (!_tableColumnInfos.TryGetValue(entityType, out List<ColumnInfo> infos))
			{
				throw new InvalidOperationException($"Table column infos don't exist for type '{entityType.Name}'.");
			}
			return infos;
		}

		public static PropertyInfo GetPropertyByStat(WeekStatType type)
		{
			if (!_weekStatProperty.TryGetValue(type, out PropertyInfo info))
			{
				throw new InvalidOperationException($"Failed to find property info mapped to week stat type '{type}'.");
			}
			return info;
		}

		// initialization
		static EntityMetadata()
		{
			ResolveMapData();
		}

		private static void ResolveMapData()
		{
			EntityTypes.ForEach(t =>
			{
				_tableNames[t] = GetTableName(t);
				_tableColumnInfos[t] = GetColumnInfos(t);

				List<string> compositeKeys = GetCompositePrimaryKeys(t);
				if (compositeKeys != null)
				{
					_compositePrimaryKeys[t] = compositeKeys;
				}
			});
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

		private static List<ColumnInfo> GetColumnInfos(Type entityType)
		{
			var result = new List<ColumnInfo>();

			List<PropertyColumnInfo> propertyColumns = GetPropertyColumns(entityType);
			result.AddRange(propertyColumns);

			List<WeekStatColumnInfo> weekStatColumns = GetWeekStatColumns(entityType);
			foreach (var c in weekStatColumns)
			{
				result.Add(c);
				_weekStatProperty[c.StatType] = c.Property;
			}

			return result;
		}

		private static List<PropertyColumnInfo> GetPropertyColumns(Type entityType)
		{
			return entityType
				.GetPropertiesContainingAttribute<ColumnAttribute>()
				.Select(PropertyColumnInfo.FromProperty)
				.ToList();
		}

		private static List<WeekStatColumnInfo> GetWeekStatColumns(Type entityType)
		{
			return entityType
				.GetPropertiesContainingAttribute<WeekStatColumnAttribute>()
				.Select(WeekStatColumnInfo.FromProperty)
				.ToList();
		}
	}
}
