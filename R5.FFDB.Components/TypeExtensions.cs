using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Components
{
	public static class TypeExtensions
	{
		public static bool IsNullable(this Type type)
		{
			return Nullable.GetUnderlyingType(type) != null;
		}

		public static bool IsNullableEnum(this Type type)
		{
			Type underlyingType = Nullable.GetUnderlyingType(type);
			return (underlyingType != null) && underlyingType.IsEnum;
		}

		public static T GetCustomAttributeOrNull<T>(this Type type)
			where T : Attribute
		{
			return type.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
		}
	}
}
