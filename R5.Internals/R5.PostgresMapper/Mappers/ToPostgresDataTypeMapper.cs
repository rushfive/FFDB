using R5.Internals.PostgresMapper.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.Internals.PostgresMapper.Mappers
{
	public static class ToPostgresDataTypeMapper
	{
		private static Dictionary<Type, PostgresDataType> _toPostgresMap = new Dictionary<Type, PostgresDataType>
		{
			{ typeof(Guid), PostgresDataType.UUID },
			{ typeof(string), PostgresDataType.TEXT },
			{ typeof(int), PostgresDataType.INT },
			{ typeof(double), PostgresDataType.FLOAT8 },
			{ typeof(DateTimeOffset), PostgresDataType.TIMESTAMPTZ },
			{ typeof(bool), PostgresDataType.BOOLEAN }
		};

		public static PostgresDataType Map<TPropertyType>()
		{
			return Map(typeof(TPropertyType));
		}

		public static PostgresDataType Map(Type propertyType)
		{
			Type underlyingType = Nullable.GetUnderlyingType(propertyType);

			if (underlyingType != null)
			{
				propertyType = underlyingType;
			}

			if (!_toPostgresMap.TryGetValue(propertyType, out PostgresDataType pgType))
			{
				throw new InvalidOperationException($"Failed to map property type '{propertyType.Name}' to a postgres data type.");
			}

			return pgType;
		}


	}
}
