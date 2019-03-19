using R5.Internals.Extensions.Reflection;
using R5.Internals.PostgresMapper.Attributes;
using R5.Internals.PostgresMapper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace R5.Internals.PostgresMapper
{
	// todo: internal
	public static class MetadataResolver
	{
		private static readonly MetadataCache _cache = new MetadataCache();

		public static string TableName<TEntity>()
		{
			return TableName(typeof(TEntity));
		}

		internal static string TableName(Type type)
		{
			var metadata = _cache.GetMetadata(type) as EntityMetadata<object>;
			return metadata.TableName;
		}

		public static List<string> CompositePrimaryKeys<TEntity>()
		{
			return CompositePrimaryKeys(typeof(TEntity));
		}

		internal static List<string> CompositePrimaryKeys(Type type)
		{
			var metadata = _cache.GetMetadata(type) as EntityMetadata<object>;
			return metadata.CompositePrimaryKeys;
		}

		public static List<TableColumn> TableColumns<TEntity>()
		{
			return TableColumns(typeof(TEntity));
		}

		internal static List<TableColumn> TableColumns(Type type)
		{
			var metadata = _cache.GetMetadata(type) as EntityMetadata<object>;
			return metadata.Columns;
		}

		internal static Dictionary<string, TableColumn> PropertyColumnMap<TEntity>()
		{
			return PropertyColumnMap(typeof(TEntity));
		}

		internal static Dictionary<string, TableColumn> PropertyColumnMap(Type type)
		{
			var metadata = _cache.GetMetadata(type) as EntityMetadata<object>;
			return metadata.Columns.ToDictionary(c => c.GetPropertyName(), c => c);
		}

		public static Func<TEntity, string> GetPrimaryKeyMatchConditionFunc<TEntity>()
		{
			return GetPrimaryKeyMatchConditionFunc(typeof(TEntity)) as Func<TEntity, string>;
		}

		internal static Func<object, string> GetPrimaryKeyMatchConditionFunc(Type type)
		{
			var metadata = _cache.GetMetadata(type) as EntityMetadata<object>;
			return metadata.GetPrimaryKeyMatchCondition;
		}
		
		class MetadataCache
		{
			private readonly Dictionary<Type, object> _metadata = new Dictionary<Type, object>();
			private readonly object _lock = new object();

			internal EntityMetadata<TEntity> GetMetadata<TEntity>()
			{
				return GetMetadata(typeof(TEntity)) as EntityMetadata<TEntity>;
			}

			internal object GetMetadata(Type type)
			{
				if (_metadata.TryGetValue(type, out object metadata))
				{
					return metadata;
				}

				lock (_lock)
				{
					if (!_metadata.ContainsKey(type))
					{
						var resolved = ResolveMetadataForEntity(type);
						_metadata[type] = resolved;
					}
				}

				return _metadata[type];
			}

			private EntityMetadata<object> ResolveMetadataForEntity(Type type)
			{
				var tableName = ResolveTableName(type);
				var compositePrimaryKeys = ResolveCompositePrimaryKeys(type);
				var columns = ResolveColumns(type);
				var getPrimaryKeyMatchCondition = ResolveGetPrimaryKeyMatchCondition(compositePrimaryKeys, columns);

				return new EntityMetadata<object>(type, tableName, compositePrimaryKeys, columns, getPrimaryKeyMatchCondition);
			}

			private string ResolveTableName(Type type)
			{
				TableAttribute attr = type.GetCustomAttributeOrNull<TableAttribute>();
				if (attr == null)
				{
					throw new InvalidOperationException($"Entity '{type.Name}' is missing its table name (add the TableAttribute to the class)");
				}

				return attr.Name;
			}

			private List<string> ResolveCompositePrimaryKeys(Type type)
			{
				CompositePrimaryKeysAttribute attr = type.GetCustomAttributeOrNull<CompositePrimaryKeysAttribute>();
				return attr?.ColumnNames;
			}

			private List<TableColumn> ResolveColumns(Type type)
			{
				List<TableColumn> columns = type.GetPropertiesContainingBaseAttribute<EntityColumnAttribute>()
					.Select(TableColumn.FromProperty)
					.ToList();

				// todo:validate columns against each other

				return columns;
			}

			private Func<object, string> ResolveGetPrimaryKeyMatchCondition(
				List<string> compositePrimaryKeys, List<TableColumn> columns)
			{
				Dictionary<string, TableColumn> columnByNameMap = columns.ToDictionary(c => c.Name, c => c, StringComparer.OrdinalIgnoreCase);
				IEnumerable<TableColumn> compositeKeyColumns = compositePrimaryKeys?.Select(c => columnByNameMap[c]);

				TableColumn primaryKeyColumn = columns.SingleOrDefault(c => c.PrimaryKey);

				if (compositeKeyColumns != null)
				{
					Debug.Assert(primaryKeyColumn == null, "Primary key column cant be set if the table uses composite primary keys.");

					return entity =>
					{
						var columnEquals = compositeKeyColumns
							.Select(c =>
							{
								var value = c.GetDbValueString(entity);
								return $"{c.Name} = {value}";
							});

						return string.Join(" AND ", columnEquals);
					};
				}
				else
				{
					//Debug.Assert(primaryKeyColumn != null, "Primary key column must be set if the table doesn't use composite primary keys.");

					return entity =>
					{
						var value = primaryKeyColumn.GetDbValueString(entity);
						return $"{primaryKeyColumn.Name} = {value}";
					};
				}
			}
		}
	}

	public class EntityMetadata<TEntity>
	{
		public Type Type { get; }
		public string TableName { get; }
		public List<string> CompositePrimaryKeys { get; }
		public List<TableColumn> Columns { get; }
		public Func<TEntity, string> GetPrimaryKeyMatchCondition { get; }

		public EntityMetadata(
			Type type,
			string tableName,
			List<string> compositePrimaryKeys,
			List<TableColumn> columns,
			Func<TEntity, string> getPrimaryKeyMatchCondition)
		{
			if (string.IsNullOrWhiteSpace(tableName))
			{
				throw new ArgumentNullException(nameof(tableName));
			}

			this.Type = type ?? throw new ArgumentNullException(nameof(type));
			this.TableName = tableName;
			this.CompositePrimaryKeys = compositePrimaryKeys;
			this.Columns = columns ?? throw new ArgumentNullException(nameof(columns));
			this.GetPrimaryKeyMatchCondition = getPrimaryKeyMatchCondition ?? throw new ArgumentNullException(nameof(getPrimaryKeyMatchCondition));
		}
	}
	
}
