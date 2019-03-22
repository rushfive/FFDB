using R5.FFDB.CLI.Commands;
using R5.FFDB.CLI.Configuration;
using R5.FFDB.CLI.Engine;
using R5.FFDB.Engine;
using System;
using System.IO;
using System.Threading.Tasks;
using static System.Console;
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

					OutputCommandInfo(runInfo);

					await new EngineRunner(engine).RunAsync(runInfo);
				}
			}
			catch (Exception ex)
			{
				CM.WriteError(ex.Message);
			}
			finally
			{
				WriteLine(Environment.NewLine + "Completed running command. Press any key to exit..");
				ReadKey();
			}
		}

		private static bool TryGetRunInfo(string[] args, out RunInfoBase runInfo)
		{
			runInfo = null;

			var runInfoBuilder = ConfigureRunInfoBuilder.Create();

			var result = runInfoBuilder.Build(args);
			if (result == null)
			{
				// version or help command
				return false;
			}

			runInfo = result as RunInfoBase;
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

		private static void OutputCommandInfo(RunInfoBase runInfo)
		{
			WriteLine(@"
   _____ _____ ____  _____ 
  |   __|   __|    \| __  |
  |   __|   __|  |  | __ -|
  |__|  |__|  |____/|_____|
             v1.0.0-alpha.1" + Environment.NewLine);

			Write("Running command: ");
			CM.WriteLineColoredReset(runInfo.CommandKey, ConsoleColor.Yellow);
			WriteLine(runInfo.Description + Environment.NewLine);
		}
	}
}
