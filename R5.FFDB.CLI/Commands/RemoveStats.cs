using R5.FFDB.Core.Models;
using R5.RunInfoBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.CLI.Commands
{
	// ffdb remove-stats all
	// ffdb remove-stats week 2018-5
	public static class RemoveStats
	{
		public class RunInfo : RunInfoBase
		{
			public WeekInfo? Week { get; set; }
		}

		internal static Command<RunInfo> Command = new Command<RunInfo>
		{
			Key = "remove-stats",
			SubCommands =
			{
				new Command<RunInfo>
				{
					Key = "all"
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
