using R5.Internals.Extensions.Reflection;
using R5.Internals.PostgresMapper.Attributes;
using R5.Internals.PostgresMapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace R5.Internals.PostgresMapper
{
	internal static class MetadataResolver
	{
		private static readonly MetadataCache _cache = new MetadataCache();

		internal static string TableName<TEntity>()
		{
			return TableName(typeof(TEntity));
		}

		internal static string TableName(Type type)
		{
			return _cache.GetTableName(type);
		}

		internal static List<string> CompositePrimaryKeys<TEntity>()
		{
			return CompositePrimaryKeys(typeof(TEntity));
		}

		internal static List<string> CompositePrimaryKeys(Type type)
		{
			return _cache.GetCompositePrimaryKeys(type);
		}
		// todo test
		internal static List<TableColumn> TableColumns<TEntity>()
		{
			return TableColumns(typeof(TEntity));
		}

		internal static List<TableColumn> TableColumns(Type type)
		{
			return _cache.GetColumns(type);
		}

		internal static Dictionary<string, TableColumn> PropertyColumnMap<TEntity>()
		{
			return PropertyColumnMap(typeof(TEntity));
		}

		internal static Dictionary<string, TableColumn> PropertyColumnMap(Type type)
		{
			List<TableColumn> columns = TableColumns(type);

			return columns.ToDictionary(c => c.GetPropertyName(), c => c);
		}

		// todo: use a richer cache model (per entity, containing for ex a map of col name to col)
		class MetadataCache
		{
			private readonly Dictionary<Type, string> _tableNames = new Dictionary<Type, string>();
			private readonly Dictionary<Type, List<string>> _compositePrimaryKeys = new Dictionary<Type, List<string>>();
			private readonly Dictionary<Type, List<TableColumn>> _columns = new Dictionary<Type, List<TableColumn>>();

			internal string GetTableName(Type type)
			{
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
		}
	}

	
}
