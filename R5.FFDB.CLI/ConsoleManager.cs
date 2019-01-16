using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.CLI
{
	public static class ConsoleManager
	{
		public static void WriteError(string message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		public static void WriteLineColored(string message, ConsoleColor color, bool reset = true)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(message);
			if (reset)
			{
				Console.ResetColor();
			}
		}
	}
}
