using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Components.Extensions.Methods
{
	public static class JObjectExtensions
	{
		public static bool TryGetToken(this JToken root, string key, out JToken token)
		{
			token = root[key];
			return token != null;
		}

		public static List<string> ChildPropertyNames(this JToken root)
		{
			return root.Children().Select(t => ((JProperty)t).Name).ToList();
		}
	}
}
