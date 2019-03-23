using R5.RunInfoBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.CLI.Commands
{
	public static class ViewState
	{
		private const string _commandKey = "view-state";

		public class RunInfo : RunInfoBase
		{
			public override string CommandKey => _commandKey;
			public override string Description => "Will acquire and display general state information regarding the app and the NFL season.";
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
