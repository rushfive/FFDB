using R5.Lib.ExtensionMethods;
using R5.PostgresMapper.Attributes;
using R5.PostgresMapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace R5.PostgresMapper
{
	internal static class MetadataResolver
	{
		private static readonly MetadataCache _cache = new MetadataCache();

		internal static string GetTableName<TEntity>()
			where TEntity : SqlEntity
		{
			return GetTableName(typeof(TEntity));
		}

		internal static string GetTableName(Type type)
		{
			return _cache.GetTableName(type);
		}

		internal static List<string> GetCompositePrimaryKeys<TEntity>()
			where TEntity : SqlEntity
		{
			return GetCompositePrimaryKeys(typeof(TEntity));
		}

		internal static List<string> GetCompositePrimaryKeys(Type type)
		{
			return _cache.GetCompositePrimaryKeys(type);
		}
		// todo test
		internal static List<TableColumn> GetColumns<TEntity>()
			where TEntity : SqlEntity
		{
			return GetColumns(typeof(TEntity));
		}

		internal static List<TableColumn> GetColumns(Type type)
		{
			return _cache.GetColumns(type);
		}

		// todo: use a richer cache model (per entity, containing for ex a map of col name to col)
		class MetadataCache
		{
			private readonly Dictionary<Type, string> _tableNames = new Dictionary<Type, string>();
			private readonly Dictionary<Type, List<string>> _compositePrimaryKeys = new Dictionary<Type, List<string>>();
			private readonly Dictionary<Type, List<TableColumn>> _columns = new Dictionary<Type, List<TableColumn>>();

			internal string GetTableName(Type type)
			{
				if (!IsSqlEntity(type))
				{
					throw new ArgumentException($"Type must derive from '{nameof(SqlEntity)}'.", nameof(type));
				}

				if (_tableNames.ContainsKey(type))
				{
					return _tableNames[type];
				}

				lock (_tableNames)
				{
					if (_tableNames.TryGetValue(type, out string tableName))
					{
						return tableName;
					}
					else
					{
						TableAttribute attr = type.GetCustomAttributeOrNull<TableAttribute>();
						if (attr == null)
						{
							throw new InvalidOperationException($"Entity '{type.Name}' is missing its table name.");
						}

						_tableNames[type] = attr.Name;
					}
				}

				return _tableNames[type];
			}

			internal List<string> GetCompositePrimaryKeys(Type type)
			{
				if (!IsSqlEntity(type))
				{
					throw new ArgumentException($"Type must derive from '{nameof(SqlEntity)}'.", nameof(type));
				}

				if (_compositePrimaryKeys.ContainsKey(type))
				{
					return _compositePrimaryKeys[type];
				}

				lock (_compositePrimaryKeys)
				{
					if (_compositePrimaryKeys.TryGetValue(type, out List<string> keys))
					{
						return keys;
					}
					else
					{
						CompositePrimaryKeysAttribute attr = type.GetCustomAttributeOrNull<CompositePrimaryKeysAttribute>();
						_compositePrimaryKeys[type] = attr?.ColumnNames;
					}
				}

				return _compositePrimaryKeys[type];
			}

			internal List<TableColumn> GetColumns(Type type)
			{
				if (!IsSqlEntity(type))
				{
					throw new ArgumentException($"Type must derive from '{nameof(SqlEntity)}'.", nameof(type));
				}

				if (_columns.ContainsKey(type))
				{
					return _columns[type];
				}

				lock (_columns)
				{
					if (_columns.TryGetValue(type, out List<TableColumn> columns))
					{
						return columns;
					}
					else
					{
						// need to validate columns against each other after creating

						columns = type.GetPropertiesContainingBaseAttribute<EntityColumnAttribute>()
							.Select(TableColumn.FromProperty)
							.ToList();

						_columns[type] = columns;
					}
				}

				return _columns[type];
			}

			private static bool IsSqlEntity(Type type)
			{
				return type.IsClass && !type.IsAbstract && type.BaseType == typeof(SqlEntity);
			}
		}
	}

	
}
