using System;
using System.Collections.Generic;
using System.Text;

namespace R5.Internals.PostgresMapper.Tests
{
	public static class StringExtensions
	{
		// doesn't remove tabs or other white space chars
		public static string RemoveWhiteSpaces(this string str)
		{
			return str.Replace(" ", String.Empty);
		}
	}
}
