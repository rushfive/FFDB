using R5.RunInfoBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.CLI.Commands
{
	// ffdb setup --force|f --config|c=path\to\config.json
	public static class InitialSetup
	{
		public class RunInfo : RunInfoBase
		{
			public bool ForceReinitialize { get; set; }
		}

		internal static Command<RunInfo> Command = new Command<RunInfo>
		{
			Key = "setup",
			Options =
			{
				new Option<RunInfo, bool>
				{
					Key = "force | f",
					Property = ri => ri.ForceReinitialize
				},
				new Option<RunInfo, string>
				{
					Key = "config | c",
					Property = ri => ri.ConfigFilePath
				}
			}
		};
	}
}
