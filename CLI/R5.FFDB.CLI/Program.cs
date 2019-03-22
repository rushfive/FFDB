using HtmlAgilityPack;
using Newtonsoft.Json;
using R5.FFDB.CLI.Commands;
using R5.FFDB.CLI.Configuration;
using R5.FFDB.CLI.Engine;
using R5.FFDB.Components.Configurations;
//using R5.FFDB.Components.CoreData.WeekStats.Models;
using R5.FFDB.DbProviders.PostgreSql.DatabaseProvider;
using R5.FFDB.Engine;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using CM = R5.Internals.Abstractions.SystemConsole.ConsoleManager;

namespace R5.FFDB.CLI
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			try
			{
				if (TryGetRunInfo(args, out RunInfoBase runInfo))
				{
					string configFilePath = GetConfigFilePath(runInfo);
					FfdbConfig config = FileConfigResolver.FromFile(configFilePath);

					FfdbEngine engine = EngineResolver.Resolve(config, runInfo);

					var runner = new EngineRunner(engine);

					await runner.RunAsync(runInfo);
				}

				return;
			}
			catch (Exception ex)
			{
				CM.WriteError(ex.Message);
				return;
			}

			Console.ReadKey();
		}

		private static bool TryGetRunInfo(string[] args, out RunInfoBase runInfo)
		{
			runInfo = null;

			var runInfoBuilder = ConfigureRunInfoBuilder.Create();

			var result = runInfoBuilder.Build(args);
			if (result == null)
			{
				// most likely version or help command
				return false;
			}

			runInfo = result as RunInfoBase;
			if (runInfo == null)
			{
				throw new InvalidOperationException("There was an error parsing program args.");
			}

			return true;
		}

		private static string GetConfigFilePath(RunInfoBase runInfo)
		{
			string path = runInfo.ConfigFilePath;
			if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
			{
				path = "ffdb_config.json";

				if (!File.Exists(path))
				{
					throw new InvalidOperationException("Failed to find config file. Ensure that you either include it as a program option "
						+ "or that the file exists in the same dir as the program exe.");
				}
			}

			return path;
		}
	}
}
