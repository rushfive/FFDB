using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace R5.FFDB.CLI.Configuration
{
	internal static class FileConfigResolver
	{
		internal static FfdbConfig FromFile(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				throw new ArgumentNullException(nameof(filePath), "File path must be provided.");
			}
			if (!File.Exists(filePath))
			{
				throw new ArgumentException($"Config file doesn't exist at path '{filePath}'.");
			}

			var settings = new JsonSerializerSettings
			{
				// prevent invalid properties on config json
				MissingMemberHandling = MissingMemberHandling.Error
			};

			FfdbConfig config;
			try
			{
				config = JsonConvert.DeserializeObject<FfdbConfig>(File.ReadAllText(filePath), settings);
			}
			catch (Exception ex)
			{
				// todo: helper/extension for easier color handling
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Failed to parse FFDB config from file.");
				Console.ResetColor();
				Console.WriteLine(ex);
				throw;
			}

			ValidateConfig(config);

			return config;
		}

		private static void ValidateConfig(FfdbConfig config)
		{
			if (string.IsNullOrWhiteSpace(config.RootDataPath))
			{
				throw new ArgumentException("Config must provide a root data directory path.");
			}

			// just set a default throttle
			if (config.WebRequest == null)
			{
				config.WebRequest = new WebRequestConfig
				{
					ThrottleMilliseconds = 1000
				};
			}

			ValidateLogging(config.Logging);
			ValidateDatabaseConfig(config);
		}

		private static void ValidateLogging(LoggingConfig config)
		{
			if (config == null)
			{
				throw new ArgumentException("Logging configuration must be provided.");
			}
			if (string.IsNullOrWhiteSpace(config.Directory))
			{
				throw new ArgumentException("Log directory must be provided.");
			}
			if (!Directory.Exists(config.Directory))
			{
				throw new ArgumentException($"Specified log directory doesn't exist at: '{config.Directory}'.");
			}

			if (string.IsNullOrWhiteSpace(config.RollingInterval)
				|| !Enum.TryParse(config.RollingInterval, out RollingInterval _))
			{
				throw new ArgumentException("Log rolling interval must be provided. "
					+ $"Valid values are: {string.Join(", ", Enum.GetNames(typeof(RollingInterval)))}");
			}

			if (string.IsNullOrWhiteSpace(config.LogLevel)
				|| !Enum.TryParse(config.LogLevel, out LogEventLevel _))
			{
				throw new ArgumentException("Log level must be provided. "
					+ $"Valid values are: {string.Join(", ", Enum.GetNames(typeof(LogEventLevel)))}");
			}
		}

		private static void ValidateDatabaseConfig(FfdbConfig config)
		{
			if (config.PostgreSql == null && config.Mongo == null)
			{
				throw new ArgumentException("Config for a database must be provided.");
			}
			if (config.PostgreSql != null && config.Mongo != null)
			{
				throw new ArgumentException("Only a single database config should be provided. "
					+ "Remove or set to null either the Postgres or Mongo configs in the json file.");
			}

			if (config.PostgreSql != null)
			{
				if (string.IsNullOrWhiteSpace(config.PostgreSql.Host))
				{
					throw new ArgumentException("Postgres host must be provided.");
				}
				if (string.IsNullOrWhiteSpace(config.PostgreSql.DatabaseName))
				{
					throw new ArgumentException("Postgres database name must be provided.");
				}
			}
			else if (config.Mongo != null)
			{
				if (string.IsNullOrWhiteSpace(config.Mongo.ConnectionString))
				{
					throw new ArgumentException("Mongo connection string must be provided.");
				}
				if (string.IsNullOrWhiteSpace(config.Mongo.DatabaseName))
				{
					throw new ArgumentException("Mongo database name must be provided.");
				}
			}
		}
	}
}
