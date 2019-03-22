using R5.FFDB.CLI.Commands;
using R5.FFDB.CLI.Configuration;
using R5.FFDB.Engine;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.CLI.Engine
{
	internal static class EngineResolver
	{
		internal static FfdbEngine Resolve(FfdbConfig config, RunInfoBase runInfo)
		{
			return new EngineSetup()
				.SetRootDataDirectoryPath(config.RootDataPath)
				.ConfigureWebClient(config)
				.ConfigureLogging(config)
				.ConfigureDbProvider(config)
				.ConfigureFromRunInfo(runInfo)
				.Create();
		}
	}

	internal static class EngineSetupExtensions
	{
		internal static EngineSetup ConfigureWebClient(this EngineSetup setup, FfdbConfig config)
		{
			if (config.WebRequest.RandomizedThrottle != null)
			{
				setup.WebRequest.SetRandomizedThrottle(
					config.WebRequest.RandomizedThrottle.Min,
					config.WebRequest.RandomizedThrottle.Max);
			}
			else
			{
				setup.WebRequest.SetThrottle(config.WebRequest.ThrottleMilliseconds);
			}

			return setup;
		}

		internal static EngineSetup ConfigureLogging(this EngineSetup setup, FfdbConfig config)
		{
			if (config.Logging != null)
			{
				var rollingInterval = config.Logging.RollingInterval.HasValue
					? config.Logging.RollingInterval.Value
					: RollingInterval.Day;

				setup.Logging
					.SetLogDirectory(config.Logging.Directory)
					.SetRollingInterval(rollingInterval);

				if (config.Logging.UseDebugLogLevel)
				{
					setup.Logging.UseDebugLogLevel();
				}

				if (config.Logging.MaxBytes.HasValue)
				{
					setup.Logging.SetMaxBytes(config.Logging.MaxBytes.Value);
				}
				if (config.Logging.RollOnFileSizeLimit)
				{
					setup.Logging.RollOnFileSizeLimit();
				}
			}

			return setup;
		}

		internal static EngineSetup ConfigureDbProvider(this EngineSetup setup, FfdbConfig config)
		{
			if (config.PostgreSql != null)
			{
				setup.UsePostgreSql(config.PostgreSql);
			}
			else if (config.Mongo != null)
			{
				setup.UseMongo(config.Mongo);
			}

			return setup;
		}

		internal static EngineSetup ConfigureFromRunInfo(this EngineSetup setup, RunInfoBase runInfo)
		{
			if (runInfo.SkipRosterFetch)
			{
				setup.SkipRosterFetch();
			}
			if (runInfo.SaveToDisk)
			{
				setup.SaveToDisk();
			}
			if (runInfo.SaveOriginalSourceFiles)
			{
				setup.SaveOriginalSourceFiles();
			}

			return setup;
		}
	}
}
