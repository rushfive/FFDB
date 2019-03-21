using System;
using System.Collections.Generic;
using System.Text;
using CM = R5.Internals.Abstractions.SystemConsole.ConsoleManager;
using static System.Console;

namespace R5.Internals.PostgresMapper
{
	internal static class DebugUtil
	{
		private static string _sqlCommandBeginBorder { get; } 
			= @"################# SQL Command #################";

		private static string _sqlCommandEndBorder { get; }
			= @"###############################################";

		internal static void OutputSqlCommand(string sqlCommand)
		{
			CM.WriteLineColored(
				Environment.NewLine + _sqlCommandBeginBorder + Environment.NewLine,
				ConsoleColor.Yellow);

			WriteLine(sqlCommand);

			CM.WriteLineColoredReset(
				Environment.NewLine + _sqlCommandEndBorder + Environment.NewLine,
				ConsoleColor.Yellow);
		}
	}
}
