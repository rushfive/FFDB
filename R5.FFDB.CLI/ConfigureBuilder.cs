using System;
using System.Collections.Generic;
using System.Text;
using R5.FFDB.CLI.Commands;
using R5.FFDB.Core.Models;
using R5.RunInfoBuilder;
using CM = R5.FFDB.CLI.ConsoleManager;

namespace R5.FFDB.CLI
{
	internal static class ConfigureBuilder
	{
		internal static RunInfoBuilder.RunInfoBuilder Get()
		{
			var builder = new RunInfoBuilder.RunInfoBuilder();

			builder.Help.DisplayOnBuildFail();

			builder.Commands
				.Add(InitialSetup.Command)
				.Add(CheckHealth.Command)
				.Add(UpdateRosters.Command)
				.Add(AddStats.Command)
				.Add(RemoveStats.Command)
				.Add(UpdatePlayers.Command);

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
	}
}
