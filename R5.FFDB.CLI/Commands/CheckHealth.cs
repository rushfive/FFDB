using R5.RunInfoBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.CLI.Commands
{
	// ffdb check-health --config|c=path\to\config.json
	public static class CheckHealth
	{
		private const string _commandKey = "check-health";
		
		public class RunInfo : RunInfoBase
		{
			public override string CommandKey => _commandKey;
		}

		internal static Command<RunInfo> Command = new Command<RunInfo>
		{
			Key = _commandKey,
			Options =
			{
				new Option<RunInfo, string>
				{
					Key = "config | c",
					Property = ri => ri.ConfigFilePath
				}
			}
		};
	}
}
