using Serilog;
using Serilog.Events;

namespace R5.FFDB.Components.Configurations
{
	public class LoggingConfig
	{
		public string LogDirectory { get; set; }
		public long? MaxBytes { get; set; }
		public RollingInterval RollingInterval { get; set; }
		public bool RollOnFileSizeLimit { get; set; }
		public LogEventLevel LogLevel { get; set; }

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
	}
}
