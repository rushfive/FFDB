using R5.RunInfoBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.CLI.Commands
{
	// ffdb update-rosters --config|c=path\to\config.json
	public static class UpdateRosters
	{
		public class RunInfo : RunInfoBase { }

		internal static Command<RunInfo> Command = new Command<RunInfo>
		{
			Key = "update-rosters",
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
