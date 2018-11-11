using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.Configurations
{
	public class LoggingConfig
	{
		public string LogDirectory { get; set; }
		public long? MaxBytes { get; set; }
		public RollingInterval RollingInterval { get; set; }
		public bool RollOnFileSizeLimit { get; set; }
		public LogEventLevel MinimumLogLevel { get; set; }
	}
}
