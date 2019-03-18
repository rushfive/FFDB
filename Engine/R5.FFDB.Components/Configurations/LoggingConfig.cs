using Serilog;
using Serilog.Events;

namespace R5.FFDB.Components.Configurations
{
	public class LoggingConfig
	{
		public string LogDirectory { get; }
		public long? MaxBytes { get; }
		public RollingInterval RollingInterval { get; }
		public bool RollOnFileSizeLimit { get; }
		public LogEventLevel LogLevel { get; }
		public Microsoft.Extensions.Logging.ILogger CustomLogger { get; }

		public bool IsConfigured => !string.IsNullOrWhiteSpace(LogDirectory);

		public LoggingConfig(
			string logDirectory,
			long? maxBytes,
			RollingInterval rollingInterval,
			bool rollOnFileSizeLimit,
			LogEventLevel logLevel)
		{
			LogDirectory = logDirectory;
			MaxBytes = maxBytes;
			RollingInterval = rollingInterval;
			RollOnFileSizeLimit = rollOnFileSizeLimit;
			LogLevel = logLevel;
		}

		private LoggingConfig(Microsoft.Extensions.Logging.ILogger logger)
		{
			CustomLogger = logger;
		}

		public static LoggingConfig Custom(Microsoft.Extensions.Logging.ILogger logger)
		{
			return new LoggingConfig(logger);
		}
	}
}
