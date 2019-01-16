using R5.FFDB.Core.Models;
using R5.RunInfoBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.CLI.Commands
{
	// ffdb update-players rostered
	// ffdb update-players all
	public static class UpdatePlayers
	{
		private const string _commandKey = "update-players";

		public class RunInfo : RunInfoBase
		{
			public override string CommandKey => _commandKey;
			public UpdatePlayersType UpdateType { get; set; }
		}

		public enum UpdatePlayersType
		{
			Rostered,
			All
		}

		internal static Command<RunInfo> Command = new Command<RunInfo>
		{
			Key = _commandKey,
			Arguments =
			{
				new SetArgument<RunInfo, UpdatePlayersType>
				{
					Property = ri => ri.UpdateType,
					Values = new List<(string, UpdatePlayersType)>
					{
						("rostered", UpdatePlayersType.Rostered),
						("all", UpdatePlayersType.All)
					}
				}
			},
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
