﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace R5.Internals.Extensions.Reflection
{
	public static class TypeExtensions
	{
		public static bool TryGetNullableUnderlyingType(this Type type, out Type underlying)
		{
			underlying = Nullable.GetUnderlyingType(type);
			return underlying != null;
		}

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

		public static List<PropertyInfo> GetPropertiesContainingAttribute<T>(this Type type)
		{
			return type
				.GetProperties()
				.Where(p => p.GetCustomAttributes()
					.Any(a => a.GetType() == typeof(T)))
				.ToList();
		}

		public static List<PropertyInfo> GetPropertiesContainingBaseAttribute<T>(this Type type)
		{
			return type.GetProperties()
				.Where(p => p.GetCustomAttributes()
					.Any(a => a.GetType().BaseType == typeof(T)))
				.ToList();
		}
	}
}
