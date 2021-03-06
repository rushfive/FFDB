﻿using R5.FFDB.Components.Configurations;
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
		private RollingInterval _rollingInterval { get; set; } = RollingInterval.Day;
		private bool _rollOnFileSizeLimit { get; set; }
		private LogEventLevel _logLevel { get; set; } = LogEventLevel.Information;
		private Microsoft.Extensions.Logging.ILogger _customLogger { get; set; }
		private string _messageTemplate { get; set; } = @"{Timestamp:MM-dd HH:mm:ss} [{PipelineStage}] {Message:lj}{NewLine}{Exception}";

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

		public LoggingConfigBuilder UseDebugLogLevel()
		{
			_logLevel = LogEventLevel.Debug;
			return this;
		}

		public LoggingConfigBuilder SetMessageTemplate(string template)
		{
			if (string.IsNullOrWhiteSpace(template))
			{
				throw new ArgumentNullException(nameof(template), "Message template must be provided.");
			}

			_messageTemplate = template;
			return this;
		}

		public void UseCustomLogger(Microsoft.Extensions.Logging.ILogger logger)
		{
			_customLogger = logger ?? throw new ArgumentNullException(nameof(logger), "Custom ILogger implementation must be provided.");
		}

		internal LoggingConfig Build()
		{
			Validate();
			
			if (_customLogger != null)
			{
				return LoggingConfig.Custom(_customLogger);
			}

			return new LoggingConfig(
				_logDirectory,
				_maxBytes,
				_rollingInterval,
				_rollOnFileSizeLimit,
				_logLevel == LogEventLevel.Debug,
				_messageTemplate);
		}

		private void Validate()
		{
			if (string.IsNullOrWhiteSpace(_logDirectory))
			{
				// not using built-in logging
				return;
			}

			if (_logLevel != LogEventLevel.Information && _logLevel != LogEventLevel.Debug)
			{
				throw new InvalidOperationException(
					$"Log level can only be set to '{LogEventLevel.Information}' or '{LogEventLevel.Debug}'.");
			}
		}
	}
}
