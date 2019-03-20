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
		public bool UseDebugLogLevel { get; }
		public Microsoft.Extensions.Logging.ILogger CustomLogger { get; }
		public string MessageTemplate { get; }

		public bool IsConfigured => !string.IsNullOrWhiteSpace(LogDirectory);

		public LogEventLevel LogLevel => UseDebugLogLevel
			? LogEventLevel.Debug
			: LogEventLevel.Information;

		public LoggingConfig(
			string logDirectory,
			long? maxBytes,
			RollingInterval rollingInterval,
			bool rollOnFileSizeLimit,
			bool useDebugLogLevel,
			string messageTemplate)
		{
			LogDirectory = logDirectory;
			MaxBytes = maxBytes;
			RollingInterval = rollingInterval;
			RollOnFileSizeLimit = rollOnFileSizeLimit;
			UseDebugLogLevel = useDebugLogLevel;
			MessageTemplate = messageTemplate;
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
