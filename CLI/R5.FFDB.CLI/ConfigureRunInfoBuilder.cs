using System;
using R5.FFDB.CLI.Commands;
using R5.FFDB.Core.Models;
using CM = R5.Internals.Abstractions.SystemConsole.ConsoleManager;
using static System.Console;

namespace R5.FFDB.CLI
{
	internal static class ConfigureRunInfoBuilder
	{
		internal static RunInfoBuilder.RunInfoBuilder Create()
		{
			var builder = new RunInfoBuilder.RunInfoBuilder();

			builder.Commands
				.Add(InitialSetup.GetCommand())
				.Add(ViewState.GetCommand())
				.Add(UpdateRosters.GetCommand())
				.Add(AddStats.GetCommand())
				.Add(UpdateRosteredPlayers.GetCommand());

			builder.Version.Set(@"
  Current version is v1.0.0-alpha.1

  For more info and docs:
  https://github.com/rushfive/FFDB");

			builder.Help
				.OnTrigger(DisplayHelp)
				.InvokeOnBuildFail(suppressException: false);

			builder.Parser.SetPredicateForType<WeekInfo?>(value =>
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					return (false, default);
				}

				string formatError = $"Failed to parse '{value}'. Ensure it's in the format 'SEASON-WEEK' eg: '2018-5' or '2018-17'.";

				var dashSplit = value.Split('-');
				if (dashSplit.Length != 2)
				{
					CM.WriteError(formatError);
					return (false, default);
				}

				if (!int.TryParse(dashSplit[0], out int season)
					|| !int.TryParse(dashSplit[1], out int week))
				{
					CM.WriteError(formatError);
					return (false, default);
				}

				return (true, new WeekInfo(season, week));
			});

			return builder;
		}

		private static void DisplayHelp()
		{
			CM.WriteLineColoredReset("Available Commands:" + Environment.NewLine, ConsoleColor.White);

			Write("┌───");
			CM.WriteLineColoredReset(" < setup >", ConsoleColor.White);
			WriteLine("│ Initializes the database and adds all available stats and data.");
			Write("│ Usage: ");
			CM.WriteLineColoredReset("ffdb setup", ConsoleColor.White);
			Write("│ Option");
			CM.WriteColoredReset(" [skip-stats] ", ConsoleColor.White);
			WriteLine("skips adding stats after db setup (ie only adds tables and static data).");
			WriteLine("└");

			Write("┌───");
			CM.WriteLineColoredReset(" < view-state >", ConsoleColor.White);
			WriteLine("│ Display general state information, such as weeks already updated.");
			Write("│ Usage: ");
			CM.WriteLineColoredReset("ffdb view-state", ConsoleColor.White);
			WriteLine("└");

			Write("┌───");
			CM.WriteLineColoredReset(" < update-rosters >", ConsoleColor.White);
			WriteLine("│ Updates players-to-team mappings.");
			Write("│ Usage: ");
			CM.WriteLineColoredReset("ffdb update-rosters", ConsoleColor.White);
			WriteLine("└");

			Write("┌───");
			CM.WriteLineColoredReset(" < add-stats >", ConsoleColor.White);
			WriteLine("│ Adds stats for players and teams (also fetches/updates relevant players).");
			WriteLine("│ Can choose between adding stats for all missing weeks, or a single specified week.");
			Write("│ Usage: ");
			CM.WriteColoredReset("ffdb add-stats missing", ConsoleColor.White);
			Write(" or ");
			CM.WriteLineColoredReset("ffdb add-stats week 2018-1", ConsoleColor.White);
			Write("│ Option");
			CM.WriteColoredReset(" [save-to-disk] ", ConsoleColor.White);
			WriteLine("saves versioned data files to disk, preventing HTTP reqs in the future.");
			Write("│ Option");
			CM.WriteColoredReset(" [save-src-files] ", ConsoleColor.White);
			WriteLine("saves original source data to disk, preventing HTTP reqs in the future.");
			WriteLine("└");

			Write("┌───");
			CM.WriteLineColoredReset(" < update-players >", ConsoleColor.White);
			WriteLine("│ Updates dynamic player info for those currently rostered.");
			Write("│ Usage: ");
			CM.WriteLineColoredReset("ffdb update-players", ConsoleColor.White);
			WriteLine("└" + Environment.NewLine);

			WriteLine("These 2 options are available for any command:");
			WriteLine("┌");
			Write("│ ");
			CM.WriteColoredReset("[config|c]", ConsoleColor.White);
			WriteLine(" path to the config file.");
			Write("│ ");
			CM.WriteColoredReset("[skip-roster|s]", ConsoleColor.White);
			WriteLine(" skips fetching roster info for every team.");
			WriteLine("│               This info is dynamic but not worth re-fetching multiple times in a day.");
			WriteLine("└" + Environment.NewLine);

			WriteLine("For better usage details and other documentation, go to:");
			WriteLine("https://github.com/rushfive/FFDB");

		}
	}
}
