using System;
using System.Collections.Generic;
using System.Text;

namespace R5.Internals.PostgresMapper
{
	// Only the specified CLR types are supported for class properties.
	// On update, make sure to update these mappers to handle them:
	// - ToDbValueStringMapper
	// - ToPostgresDataTypeMapper
	// - DbValueToObjectMapper
	internal static class SupportedTypes
	{
		private static HashSet<Type> _types = new HashSet<Type>
		{
			typeof(Guid),
			typeof(string),
			typeof(int),
			typeof(double),
			typeof(DateTimeOffset),
			typeof(bool)
		};

		internal static bool IsSupported<T>()
		{
			return _types.Contains(typeof(T));
		}
	}
}
