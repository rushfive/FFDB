using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.Internals.Extensions.Collections
{
	public static class ListExtensions
	{
		public static bool IsNullOrEmpty<T>(this List<T> list)
		{
			return list == null || !list.Any();
		}
	}
}
