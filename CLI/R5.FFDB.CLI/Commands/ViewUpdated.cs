﻿using R5.RunInfoBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.CLI.Commands
{
	// ffdb view-updated --config|c=path\to\config.json
	public static class ViewUpdated
	{
		private const string _commandKey = "view-updated";

		public class RunInfo : RunInfoBase
		{
			public override string CommandKey => _commandKey;
		}

		internal static Command<RunInfo> GetCommand()
		{
			var command = new Command<RunInfo>
			{
				Key = _commandKey
			};

			RunInfoBase.AddCommonOptions(command);
			return command;
		}
	}
}
