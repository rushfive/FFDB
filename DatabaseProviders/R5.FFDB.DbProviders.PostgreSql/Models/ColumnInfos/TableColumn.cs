using R5.Lib.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace R5.FFDB.DbProviders.PostgreSql.Models.ColumnInfos
{
	// abstraction representing the mapping between a table column
	// and the C# model's property. Contains methods enabling
	// reading/writing between the two
	public abstract class TableColumn
	{
		public string Name { get; }
		public PostgresDataType DataType { get; }
		private PropertyInfo _property { get; }

		protected TableColumn(
			string name,
			PostgresDataType dataType,
			PropertyInfo property)
		{
			Name = name;
			DataType = dataType;
			_property = property;
		}

		internal abstract string GetSqlColumnDefinition();

		internal string GetPropertyName()
		{
			return _property.Name;
		}

		internal object GetValue(object obj)
		{
			return _property.GetValue(obj);
		}

		internal void SetValue(object obj, object value)
		{
			object resolvedValue = _property.PropertyType.IsNullable()
				? ResolveValueForNullable(value)
				: ResolveValue(value);

			_property.SetValue(obj, resolvedValue);
		}

		private object ResolveValueForNullable(object value)
		{
			if (value == null || value.GetType() == typeof(DBNull))
			{
				return null;
			}

			Type type = _property.PropertyType;

			if (type.IsNullableEnum())
			{
				Type nullableType = Nullable.GetUnderlyingType(type);
				return Enum.Parse(nullableType, (string)value);
			}

			return ResolveValue(value);
		}

		private object ResolveValue(object value)
		{
			Type type = _property.PropertyType;
			object converted = value;

			switch (DataType)
			{
				case PostgresDataType.TEXT:
					if (type.IsEnum)
					{
						converted = Enum.Parse(type, (string)value);
					}
					break;
				case PostgresDataType.DATE:
				case PostgresDataType.TIMESTAMPTZ:
					DateTime dt = (DateTime)value;
					dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);

					DateTimeOffset dtOffset = dt;
					converted = dtOffset;
					break;
				case PostgresDataType.UUID:
				case PostgresDataType.INT:
				case PostgresDataType.FLOAT8:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(DataType), $"'{DataType}' is an invalid '{nameof(PostgresDataType)}'.");
			}

			return converted;
		}
	}
}
