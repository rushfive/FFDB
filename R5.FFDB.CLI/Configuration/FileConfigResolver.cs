using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CM = R5.FFDB.CLI.ConsoleManager;

namespace R5.FFDB.CLI.Configuration
{
	internal static class FileConfigResolver
	{
		internal static FfdbConfig FromFile(string filePath)
		{
			// default search for file in same directory as program
			if (string.IsNullOrWhiteSpace(filePath))
			{
				filePath = Path.Combine(Directory.GetCurrentDirectory(), "ffdb_config.json");
			}

			if (!File.Exists(filePath))
			{
				throw new ArgumentException($"FFDB config file doesn't exist at '{filePath}'.");
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
				throw new InvalidOperationException("Failed to parse FFDB config from file.", ex);
			}

			config.ThrowIfInvalid();

			return config;
		}
	}
}
