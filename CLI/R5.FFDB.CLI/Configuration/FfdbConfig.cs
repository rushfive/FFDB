﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using R5.FFDB.DbProviders.Mongo;
using R5.FFDB.DbProviders.PostgreSql.DatabaseProvider;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace R5.FFDB.CLI.Configuration
{
	public class FfdbConfig
	{
		public string RootDataPath { get; set; }
		public WebRequestConfig WebRequest { get; set; }
		public LoggingConfig Logging { get; set; }
		public PostgresConfig PostgreSql { get; set; }
		public MongoConfig Mongo { get; set; }

		public void ThrowIfInvalid()
		{
			if (string.IsNullOrWhiteSpace(RootDataPath))
			{
				throw new ArgumentException("Config must provide a root data directory path.");
			}

			ValidateWebRequest();
			ValidateLogging();
			ValidateDatabaseConfig();
		}

		private void ValidateWebRequest()
		{
			if (WebRequest == null)
			{
				WebRequest = new WebRequestConfig
				{
					ThrottleMilliseconds = 1000
				};
				return;
			}

			if (WebRequest.ThrottleMilliseconds < 0)
			{
				throw new ArgumentException("Web request throttle must be a value greater than or equal to 0.");
			}
			if (WebRequest.RandomizedThrottle != null)
			{
				if (WebRequest.RandomizedThrottle.Min < 0
					|| WebRequest.RandomizedThrottle.Max < 0)
				{
					throw new ArgumentException("Web request randomized throttle values must be greater than or equal to 0.");
				}
				if (WebRequest.RandomizedThrottle.Min >= WebRequest.RandomizedThrottle.Max)
				{
					throw new ArgumentException("Web request randomized throttle min value must be less than the max.");
				}
			}
		}

		private void ValidateLogging()
		{
			if (string.IsNullOrWhiteSpace(Logging?.Directory))
			{
				return;
			}
			if (!Directory.Exists(Logging.Directory))
			{
				Directory.CreateDirectory(Logging.Directory);
			}
		}

		private void ValidateDatabaseConfig()
		{
			if (PostgreSql == null && Mongo == null)
			{
				throw new ArgumentException("Config for a database must be provided.");
			}
			if (PostgreSql != null && Mongo != null)
			{
				throw new ArgumentException("Only a single database config should be provided. "
					+ "Remove or set to null either the Postgres or Mongo configs in the json file.");
			}

			if (PostgreSql != null)
			{
				if (string.IsNullOrWhiteSpace(PostgreSql.Host))
				{
					throw new ArgumentException("Postgres host must be provided.");
				}
				if (string.IsNullOrWhiteSpace(PostgreSql.DatabaseName))
				{
					throw new ArgumentException("Postgres database name must be provided.");
				}
			}
			else if (Mongo != null)
			{
				if (string.IsNullOrWhiteSpace(Mongo.ConnectionString))
				{
					throw new ArgumentException("Mongo connection string must be provided.");
				}
				if (string.IsNullOrWhiteSpace(Mongo.DatabaseName))
				{
					throw new ArgumentException("Mongo database name must be provided.");
				}
			}
		}
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

		[JsonConverter(typeof(StringEnumConverter))]
		public RollingInterval? RollingInterval { get; set; }

		public bool RollOnFileSizeLimit { get; set; }
		public bool UseDebugLogLevel { get; set; }
	}
}
