using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Attributes;
using R5.FFDB.DbProviders.PostgreSql.Models;
using R5.FFDB.DbProviders.PostgreSql.Models.ColumnInfos;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
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
		private static Dictionary<Type, string> _tableNames { get; } = new Dictionary<Type, string>();
		private static Dictionary<Type, List<ColumnInfo>> _tableColumnInfos { get; } = new Dictionary<Type, List<ColumnInfo>>();
		private static Dictionary<WeekStatType, PropertyInfo> _weekStatProperty { get; } = new Dictionary<WeekStatType, PropertyInfo>();

		private static List<Type> _entityTypes = new List<Type>
		{
			typeof(TeamSql),
			typeof(PlayerSql),
			typeof(PlayerTeamMapSql),
			typeof(WeekStatsSql),
			typeof(WeekStatsKickerSql),
			typeof(WeekStatsDstSql),
			typeof(WeekStatsIdpSql)
		};

		private static HashSet<Type> _baseEntityTypes = new HashSet<Type>
		{
			typeof(SqlEntity),
			typeof(WeekStatsSqlBase)
		};

		public static string TableName(Type entityType)
		{
			if (!_tableNames.TryGetValue(entityType, out string name))
			{
				throw new InvalidOperationException($"Table name doesn't exist for type '{entityType.Name}'.");
			}

			return name;
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

		static EntityInfoMap()
		{
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
			if (!_baseEntityTypes.Contains(entityType.BaseType))
			{
				// is this too restrictive?
				throw new ArgumentException($"EntityInfoMap should only deal with types deriving from '{typeof(SqlEntity).Name}'.");
			}

			CustomAttributeData attr = entityType.CustomAttributes
				.SingleOrDefault(a => a.AttributeType == typeof(TableNameAttribute));

			if (attr == null)
			{
				throw new InvalidOperationException($"Entity '{entityType.Name}' is missing its table name.");
			}

			return (string)attr.ConstructorArguments[0].Value;
		}

		private static List<ColumnInfo> GetColumnInfos(Type entityType)
		{
			List<PropertyInfo> columnProperties = entityType
				.GetProperties()
				.Where(p => p.GetCustomAttributes()
					.Any(a => a.GetType().BaseType == typeof(EntityColumnAttribute)))
				.ToList();

			if (!columnProperties.Any())
			{
				throw new InvalidOperationException($"Type '{entityType.Name}' doesn't contain any column attributes.");
			}

			var result = new List<ColumnInfo>();

			foreach (PropertyInfo property in columnProperties)
			{
				ColumnInfo info = ColumnInfoResolver.FromProperty(property);
				result.Add(info);

				if (info is WeekStatColumnInfo weekStatInfo)
				{
					_weekStatProperty[weekStatInfo.StatType] = weekStatInfo.Property;
				}
			}

			return result;
		}
	}
}
