using R5.Internals.PostgresMapper.Models;
using R5.Lib.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.Internals.PostgresMapper.Mappers
{
	internal static class DbValueToObjectMapper
	{
		internal static object Map(object value,
			Type propertyType, PostgresDataType dataType)
		{
			if (propertyType.IsNullable())
			{
				return ResolveValueForNullable(value, propertyType, dataType);
			}

			return ResolveValue(value, propertyType, dataType);
		}

		private static object ResolveValueForNullable(object value,
			Type propertyType, PostgresDataType dataType)
		{
			if (value == null || value.GetType() == typeof(DBNull))
			{
				return null;
			}

			if (propertyType.IsNullableEnum())
			{
				Type nullableType = Nullable.GetUnderlyingType(propertyType);
				return Enum.Parse(nullableType, (string)value);
			}

			return ResolveValue(value, propertyType, dataType);
		}

		private static object ResolveValue(object value,
			Type propertyType, PostgresDataType dataType)
		{
			object converted = value;

			switch (dataType)
			{
				case PostgresDataType.TEXT:
					if (propertyType.IsEnum)
					{
						converted = Enum.Parse(propertyType, (string)value);
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
					throw new ArgumentOutOfRangeException(nameof(dataType), $"'{dataType}' is an invalid '{nameof(PostgresDataType)}'.");
			}

			return converted;
		}
	}
}
