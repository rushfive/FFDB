using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;
using CM = R5.Internals.Abstractions.SystemConsole.ConsoleManager;

namespace R5.Internals.Abstractions.SystemConsole
{
	public static class ConsoleManager
	{
		public static void WriteLineColored(string value, ConsoleColor color)
		{
			ForegroundColor = color;
			WriteLine(value);
		}

		public static void WriteLineColoredReset(string value, ConsoleColor color)
		{
			CM.WriteLineColored(value, color);
			ResetColor();
		}
	}
}
