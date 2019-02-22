using R5.Internals.PostgresMapper.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.Internals.PostgresMapper
{
	internal static class Extensions
	{
		internal static bool IsSqlEntity(this Type type)
		{
			return type.IsClass && !type.IsAbstract && type.BaseType == typeof(SqlEntity);
		}

		internal static void ThrowIfNotSqlEntity(this Type type)
		{
			if (!type.IsSqlEntity())
			{
				throw new ArgumentException($"Type must derive from '{nameof(SqlEntity)}'.", nameof(type));
			}
		}
	}
}
