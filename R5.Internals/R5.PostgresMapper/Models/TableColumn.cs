using R5.Lib.ExtensionMethods;
using R5.Internals.PostgresMapper.Attributes;
using R5.Internals.PostgresMapper.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace R5.Internals.PostgresMapper.Models
{
	public class TableColumn
	{
		public string Name { get; private set; }
		public PostgresDataType DataType { get; private set; }
		public bool PrimaryKey { get; private set; }
		public bool NotNull { get; private set; }
		public Type ForeignTableType { get; private set; }
		public string ForeignKeyColumn { get; private set; }
		public Type PropertyType { get; private set; }

		public bool HasForeignKeyConstraint =>
			ForeignTableType != null && !string.IsNullOrWhiteSpace(ForeignKeyColumn);

		private PropertyInfo _property { get; }
		
		public void SetAsPrimaryKey() => PrimaryKey = true;
		public void SetAsNotNull() => NotNull = true;
		public void SetForeignTableType(Type type) => ForeignTableType = type;
		public void SetForeignKeyColumn(string columnName) => ForeignKeyColumn = columnName;
		public void SetPropertyType(Type type) => PropertyType = type;

		public TableColumn(PropertyInfo property)
		{
			PropertyType = property.PropertyType;
			_property = property;
		}
		

		public static TableColumn FromProperty(PropertyInfo property)
		{
			var column = new TableColumn(property);

			var columnAttr = property.GetCustomAttribute<ColumnAttribute>();
			SetNameDataType(column, columnAttr);

			foreach(var attr in property.GetCustomAttributes<EntityColumnAttribute>())
			{
				attr.UpdateTableColumn(column);
			}

			return column;
		}

		private static void SetNameDataType(TableColumn column, ColumnAttribute attr)
		{
			string name = attr != null && !string.IsNullOrWhiteSpace(attr.Name)
				? attr.Name
				: column.PropertyType.Name;

			PostgresDataType dataType = attr != null && attr.DataType.HasValue
				? attr.DataType.Value
				: ToPostgresDataTypeMapper.Map(column.PropertyType);

			column.Name = name;
			column.DataType = dataType;
		}

		public string GetPropertyName()
		{
			return _property.Name;
		}

		public object GetValue(object obj)
		{
			return _property.GetValue(obj);
		}

		public void SetValue(object obj, object value)
		{
			object resolvedValue = DbValueToObjectMapper.Map(value, _property.PropertyType, DataType);

			if (NotNull && resolvedValue == null)
			{
				throw new InvalidOperationException($"Column '{Name}' requires a value but was tried to set as null.");
			}

			_property.SetValue(obj, resolvedValue);
		}
	}
}
