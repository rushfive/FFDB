using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.CLI.Configuration
{
	public class FfdbConfig
	{
		public string RootDataPath { get; set; }
		public WebRequestConfig WebRequest { get; set; }
		public LoggingConfig Logging { get; set; }
		public PostgreSqlConfig PostgreSql { get; set; }
		public MongoConfig Mongo { get; set; }
	}

	public class WebRequestConfig
	{
		public int ThrottleMilliseconds { get; set; }
		public RandomizedThrottleConfig RandomizedThrottle { get; set; }

		public class RandomizedThrottleConfig
		{
			public int Min { get; set; }
			public int Max { get; set; }
		}
	}

	public class LoggingConfig
	{
		public string Directory { get; set; }
		public long? MaxBytes { get; set; }

		// Serilog.RollingInterval
		public string RollingInterval { get; set; }// = RollingInterval.Day;
		public bool RollOnFileSizeLimit { get; set; }

		// Serilog.Events.LogEventLevel
		public string LogLevel { get; set; }// = LogEventLevel.Debug;
	}

	public class PostgreSqlConfig
	{
		public string Host { get; set; }
		public string DatabaseName { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public class MongoConfig
	{
		public string ConnectionString { get; set; }
		public string DatabaseName { get; set; }
	}
}
