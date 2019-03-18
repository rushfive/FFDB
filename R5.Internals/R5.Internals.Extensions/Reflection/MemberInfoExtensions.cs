using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace R5.Internals.Extensions.Reflection
{
	public static class MemberInfoExtensions
	{
		public static Type GetUnderlyingType(this MemberInfo member)
		{
			switch (member.MemberType)
			{
				case MemberTypes.Event:
					return ((EventInfo)member).EventHandlerType;
				case MemberTypes.Field:
					return ((FieldInfo)member).FieldType;
				case MemberTypes.Method:
					return ((MethodInfo)member).ReturnType;
				case MemberTypes.Property:
					return ((PropertyInfo)member).PropertyType;
				default:
					throw new ArgumentException($"'{member.MemberType}' is invalid or unhandled.", nameof(member.MemberType));
			}
		}

		public static bool TryGetPropertyType(this MemberInfo member, out Type type)
		{
			type = null;

			if (member.MemberType == MemberTypes.Property)
			{
				type = ((PropertyInfo)member).PropertyType;
				return true;
			}

			return false;
		}
	}
}
