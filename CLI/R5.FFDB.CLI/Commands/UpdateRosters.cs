using R5.RunInfoBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.CLI.Commands
{
	public static class UpdateRosters
	{
		private const string _commandKey = "update-rosters";

		public class RunInfo : RunInfoBase
		{
			public override string CommandKey => _commandKey;
			public override string Description => "Updates mappings between players-and-teams, based off of a current fetch of team roster information.";
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
