using R5.FFDB.Core.Models;
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

		internal static Command<RunInfo> Command = new Command<RunInfo>
		{
			Key = _commandKey,
			SubCommands =
			{
				new Command<RunInfo>
				{
					Key = "missing"
				},
				new Command<RunInfo>
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
			}
		};
	}
}
