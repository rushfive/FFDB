using R5.Internals.PostgresMapper.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace R5.Internals.PostgresMapper.Mappers
{
	internal static class ToDbValueStringMapper
	{
		private static Dictionary<Type, Func<object, string>> _converters = new Dictionary<Type, Func<object, string>>
		{
			{ typeof(Guid), FromGuid },
			{ typeof(string), FromString },
			{ typeof(int), FromNumeric },
			{ typeof(double), FromNumeric },
			{ typeof(DateTimeOffset), DateTimeOffsetToTimeStampTz },
			{ typeof(bool), FromBool },
		};

		internal static string Map(object value, PostgresDataType dataType)
		{
			if (value == null)
			{
				return "null";
			}

			switch (dataType)
			{
				case PostgresDataType.UUID:
					return _converters[typeof(Guid)](value);
				case PostgresDataType.TEXT:
					return _converters[typeof(string)](value);
				case PostgresDataType.INT:
				case PostgresDataType.FLOAT8:
					return _converters[typeof(int)](value);
				case PostgresDataType.DATE:
					return $"'{((DateTimeOffset)value).ToUniversalTime().ToString("yyyy-MM-dd")}'";
				case PostgresDataType.TIMESTAMPTZ:
					return _converters[typeof(DateTimeOffset)](value);
				case PostgresDataType.BOOLEAN:
					return _converters[typeof(bool)](value);
				default:
					throw new ArgumentOutOfRangeException($"'{dataType}' is not a valid postgres data type.");
			}

			//if (column.DataType.HasValue)
			//{
			//	return ResolveByDataType(value, column.DataType.Value);
			//}

			//return ResolveByPropertyType(value, column);
		}

		private static string ResolveByDataType(object value, PostgresDataType dataType)
		{
			switch (dataType)
			{
				case PostgresDataType.UUID:
					return _converters[typeof(Guid)](value);
				case PostgresDataType.TEXT:
					return _converters[typeof(string)](value);
				case PostgresDataType.INT:
				case PostgresDataType.FLOAT8:
					return _converters[typeof(int)](value);
				case PostgresDataType.DATE:
					return $"'{((DateTimeOffset)value).ToUniversalTime().ToString("yyyy-MM-dd")}'";
				case PostgresDataType.TIMESTAMPTZ:
					return _converters[typeof(DateTimeOffset)](value);
				case PostgresDataType.BOOLEAN:
					return _converters[typeof(bool)](value);
				default:
					throw new ArgumentOutOfRangeException($"'{dataType}' is not a valid postgres data type.");
			}
		}

		private static string ResolveByPropertyType(object value, TableColumn column)
		{
			Type type = Nullable.GetUnderlyingType(column.PropertyType);

			if (type == null)
			{
				type = column.PropertyType;
			}

			return _converters[type](value);
		}

		private static string FromGuid(object guid)
		{
			return $"'{guid}'";
		}

		private static string FromString(object str)
		{
			return $"'{str.ToString().Replace("'", "''")}'";
		}

		// handles int/double
		private static string FromNumeric(object num)
		{
			return num.ToString();
		}

		private static string DateTimeOffsetToDate(object dtOffset)
		{
			return $"'{((DateTimeOffset)dtOffset).ToUniversalTime().ToString("yyyy-MM-dd")}'";
		}

		private static string DateTimeOffsetToTimeStampTz(object dtOffset)
		{
			return $"'{((DateTimeOffset)dtOffset).ToUniversalTime().ToString("o", CultureInfo.InvariantCulture)}'";
		}

		private static string FromBool(object boolean)
		{
			return (bool)boolean ? "true" : "false";
		}
	}
}
