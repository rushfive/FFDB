using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData.Static.ReceiverTargets.PlayerMatcher
{
	public static class EditDistance
	{
		public static int Find(string s1, string s2)
		{
			var memo = new Dictionary<string, int>();
			return FindRecurse(s1, s2, 0, 0, memo);
		}

		private static int FindRecurse(string s1, string s2,
				int i1, int i2, Dictionary<string, int> memo)
		{
			string key = $"{i1}-{i2}";

			if (memo.ContainsKey(key))
			{
				return memo[key];
			}

			if (i1 == s1.Length)
			{
				return s2.Length - i2;
			}
			if (i2 == s2.Length)
			{
				return s1.Length - i1;
			}

			int minOps;
			if (s1[i1] == s2[i2])
			{
				minOps = FindRecurse(s1, s2, i1 + 1, i2 + 1, memo);
			}
			else
			{
				int delete = FindRecurse(s1, s2, i1 + 1, i2, memo);
				int insert = FindRecurse(s1, s2, i1, i2 + 1, memo);
				int replace = FindRecurse(s1, s2, i1 + 1, i2 + 1, memo);

				minOps = 1 + Math.Min(delete, Math.Min(insert, replace));
			}

			memo[key] = minOps;
			return minOps;
		}
	}
}
