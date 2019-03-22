using R5.RunInfoBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.CLI.Commands
{
	// ffdb setup --skip-stats|s --config|c=path\to\config.json
	public static class InitialSetup
	{
		private const string _commandKey = "setup";

		public class RunInfo : RunInfoBase
		{
			public override string CommandKey => _commandKey;
			public bool SkipAddingStats { get; set; }
		}

		internal static Command<RunInfo> GetCommand()
		{
			var command = new Command<RunInfo>
			{
				Key = _commandKey,
				Options =
				{
					new Option<RunInfo, bool>
					{
						Key = "skip-stats | s",
						Property = ri => ri.SkipAddingStats
					}
				}
			};

			RunInfoBase.AddCommonOptions(command);
			return command;
		}
	}
}
