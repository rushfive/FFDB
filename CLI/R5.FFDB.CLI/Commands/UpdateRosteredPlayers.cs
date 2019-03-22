using R5.FFDB.Core.Models;
using R5.RunInfoBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.CLI.Commands
{
	// ffdb update-players rostered
	// ffdb update-players all
	public static class UpdateRosteredPlayers
	{
		private const string _commandKey = "update-players";

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
