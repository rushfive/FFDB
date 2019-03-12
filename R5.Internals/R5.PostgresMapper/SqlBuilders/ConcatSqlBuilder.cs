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

		public string GetResult(bool omitTerminatingSemiColon = false)
		{
			var result = _sb.ToString().Trim();

			if (string.IsNullOrWhiteSpace(result))
			{
				throw new InvalidOperationException("SQL result is invalid: cannot be null or empty.");
			}

			if (omitTerminatingSemiColon && result.EndsWith(";"))
			{
				throw new InvalidOperationException("Cannot omit terminating semi-colon because the SQL command already contains one.");
			}

			if (!omitTerminatingSemiColon)
			{
				return result.EndsWith(";") ? result : result + ";";
			}

			return result;
		}

		public override string ToString()
		{
			var result = _sb.ToString().Trim();

			return string.IsNullOrWhiteSpace(result) ? "[empty]" : result;
		}
	}
}
