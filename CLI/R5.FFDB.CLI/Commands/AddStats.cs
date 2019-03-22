﻿using R5.FFDB.Core.Models;
using R5.RunInfoBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.CLI.Commands
{
	// ffdb add-stats missing
	// ffdb add-stats week 2018-5
	public static class AddStats
	{
		private const string _commandKey = "add-stats";

		public class RunInfo : RunInfoBase
		{
			public override string CommandKey => _commandKey;
			public WeekInfo? Week { get; set; }
		}

		internal static Command<RunInfo> GetCommand()
		{
			var command = new Command<RunInfo>
			{
				Key = _commandKey,
				SubCommands =
				{
					new SubCommand<RunInfo>
					{
						Key = "missing"
					},
					new SubCommand<RunInfo>
					{
						Key = "week",
						Arguments =
						{
							new PropertyArgument<RunInfo, WeekInfo?>
							{
								Property = ri => ri.Week,
								HelpToken = "<week>"
							}
						}
					}
				},
				GlobalOptions =
				{
					new Option<RunInfo, bool>
					{
						Key = "save-to-disk",
						Property = ri => ri.SaveToDisk
					},
					new Option<RunInfo, bool>
					{
						Key = "save-src-files",
						Property = ri => ri.SaveOriginalSourceFiles
					}
				}
			};

			RunInfoBase.AddCommonOptions(command);
			return command;
		}
	}
}
