using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components
{
	public static class TypeExtensions
	{
		public static bool IsNullable(this Type t)
		{
			return Nullable.GetUnderlyingType(t) != null;
		}

		public static bool IsNullableEnum(this Type t)
		{
			Type u = Nullable.GetUnderlyingType(t);
			return (u != null) && u.IsEnum;
		}
	}
}
