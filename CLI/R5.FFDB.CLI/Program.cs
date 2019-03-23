using Newtonsoft.Json;
using R5.FFDB.CLI.Commands;
using R5.FFDB.CLI.Configuration;
using R5.FFDB.CLI.Engine;
using R5.FFDB.Engine;
using System;
using System.IO;
using System.Net.Http;
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

					DataRepoState dataRepoState = await GetDataRepoStateAsync();

					FfdbEngine engine = EngineResolver.Resolve(config, runInfo, dataRepoState);

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

		private static async Task<DataRepoState> GetDataRepoStateAsync()
		{
			try
			{
				using (var client = new HttpClient())
				{
					string uri = @"https://raw.githubusercontent.com/rushfive/FFDB.Data/master/state.json";

					string response = await client.GetStringAsync(uri);

					return JsonConvert.DeserializeObject<DataRepoState>(response);
				}
			}
			catch (Exception ex)
			{
				CM.WriteError("Failed to fetch data repo state, this feature will be disabled."
					+ Environment.NewLine + ex);
				return null;
			}
			
		}

		private static void OutputCommandInfo(RunInfoBase runInfo)
		{
			CM.WriteLineColoredReset(@"
   _____ _____ ____  _____ 
  |   __|   __|    \| __  |
  |   __|   __|  |  | __ -|
  |__|  |__|  |____/|_____|", ConsoleColor.Cyan);

			CM.WriteLineColoredReset("             v1.0.0-alpha.1" + Environment.NewLine, ConsoleColor.White);

			Write("Running command: ");
			CM.WriteLineColoredReset(runInfo.CommandKey, ConsoleColor.Yellow);
			WriteLine(runInfo.Description + Environment.NewLine);
		}
	}
}
