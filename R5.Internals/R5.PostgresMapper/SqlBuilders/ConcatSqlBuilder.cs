using System;
using System.Collections.Generic;
using System.Text;

namespace R5.Internals.PostgresMapper.SqlBuilders
{
	public class ConcatSqlBuilder
	{
		private readonly StringBuilder _sb = new StringBuilder();

		public ConcatSqlBuilder Append(string sql)
		{
			var handledForSpacing = sql.Trim() + " ";
			_sb.Append(handledForSpacing);

			return this;
		}

		public string GetResult()
		{
			var result = _sb.ToString().Trim();

			if (string.IsNullOrWhiteSpace(result))
			{
				throw new InvalidOperationException("SQL result is invalid: cannot be null or empty.");
			}

			return result.EndsWith(";") ? result : result + ";";
		}

		public override string ToString()
		{
			var result = _sb.ToString().Trim();

			return string.IsNullOrWhiteSpace(result) ? "[empty]" : result;
		}
	}
}
