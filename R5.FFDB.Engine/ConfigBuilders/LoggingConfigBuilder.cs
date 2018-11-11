using R5.FFDB.Components.Configurations;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace R5.FFDB.Engine.ConfigBuilders
{
	public class LoggingConfigBuilder
	{
		private string _logDirectory { get; set; }
		private long? _maxBytes { get; set; }
		private RollingInterval _rollingInterval { get; set; } = RollingInterval.Hour;
		private bool _rollOnFileSizeLimit { get; set; }
		private LogEventLevel _logLevel { get; set; } = LogEventLevel.Debug;

		public LoggingConfigBuilder SetLogDirectory(string directoryPath)
		{
			if (string.IsNullOrWhiteSpace(directoryPath))
			{
				throw new ArgumentNullException(nameof(directoryPath), "Logging directory path must be provided.");
			}
			if (!directoryPath.EndsWith("\\"))
			{
				directoryPath += "\\";
			}
			if (!Directory.Exists(directoryPath))
			{
				throw new ArgumentException($"Directory path '{directoryPath}' doesn't exist.");
			}

			_logDirectory = directoryPath;
			return this;
		}

		public LoggingConfigBuilder SetMaxBytes(long maxBytes)
		{
			if (maxBytes <= 0)
			{
				throw new ArgumentException("Max bytes value must be greater than 0.");
			}

			_maxBytes = maxBytes;
			return this;
		}

		public LoggingConfigBuilder SetRollingInterval(RollingInterval interval)
		{
			_rollingInterval = interval;
			return this;
		}

		public LoggingConfigBuilder RollOnFileSizeLimit()
		{
			_rollOnFileSizeLimit = true;
			return this;
		}

		public LoggingConfigBuilder SetLogLevel(LogEventLevel level)
		{
			_logLevel = level;
			return this;
		}

		internal LoggingConfig Build()
		{
			Validate();

			return new LoggingConfig(
				_logDirectory,
				_maxBytes,
				_rollingInterval,
				_rollOnFileSizeLimit,
				_logLevel);
		}

		private void Validate()
		{
			if (string.IsNullOrWhiteSpace(_logDirectory))
			{
				throw new InvalidOperationException("Logging directory must be provided.");
			}
		}
	}
}
