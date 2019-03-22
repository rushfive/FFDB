using R5.FFDB.CLI.Configuration;
using R5.FFDB.DbProviders.PostgreSql.DatabaseProvider;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace R5.FFDB.CLI.Tests.Tests
{
	public class FfdbConfigValidationTests
	{
		private static FfdbConfig GetValidConfig()
		{
			return new FfdbConfig
			{
				RootDataPath = @"c:\config.json",
				Logging = new LoggingConfig
				{
					Directory = @"c:\log.txt"
				},
				WebRequest = new WebRequestConfig
				{
					RandomizedThrottle = new WebRequestConfig.RandomizedThrottleConfig
					{
						Max = 2000,
						Min = 1000
					},
					ThrottleMilliseconds = 1000
				},
				Mongo = new DbProviders.Mongo.MongoConfig
				{
					ConnectionString = "mongo_connection_str",
					DatabaseName = "database_name"
				}
			};
		}

		[Fact]
		public void Config_FromHelperFactoryMethod_IsValid()
		{
			var config = GetValidConfig();
			config.ThrowIfInvalid();
		}

		[Fact]
		public void ThrottleMilliseconds_UnderZero_Throws()
		{
			var config = GetValidConfig();

			config.WebRequest.RandomizedThrottle = null;
			config.WebRequest.ThrottleMilliseconds = -1;

			Assert.Throws<ArgumentException>(() => config.ThrowIfInvalid());
		}

		[Fact]
		public void RandomizedThrottle_Min_UnderZero_Throws()
		{
			var config = GetValidConfig();

			config.WebRequest.RandomizedThrottle.Min = -1;

			Assert.Throws<ArgumentException>(() => config.ThrowIfInvalid());
		}

		[Fact]
		public void RandomizedThrottle_Max_UnderZero_Throws()
		{
			var config = GetValidConfig();

			config.WebRequest.RandomizedThrottle.Max = -1;

			Assert.Throws<ArgumentException>(() => config.ThrowIfInvalid());
		}

		[Fact]
		public void RandomizedThrottle_Min_GreaterThan_Max_Throws()
		{
			var config = GetValidConfig();

			config.WebRequest.RandomizedThrottle.Max = 1;
			config.WebRequest.RandomizedThrottle.Min = 1;

			Assert.Throws<ArgumentException>(() => config.ThrowIfInvalid());
		}

		[Fact]
		public void PostgresAndMongo_NotConfigured_Throws()
		{
			var config = GetValidConfig();

			config.Mongo = null;
			config.PostgreSql = null;

			Assert.Throws<ArgumentException>(() => config.ThrowIfInvalid());
		}

		[Fact]
		public void PostgresAndMongo_BothConfigured_Throws()
		{
			var config = GetValidConfig();
			
			config.PostgreSql = new PostgresConfig
			{
				Host = "host", DatabaseName = "db"
			};

			Assert.Throws<ArgumentException>(() => config.ThrowIfInvalid());
		}

		[Fact]
		public void Postgres_Host_NotConfigured_Throws()
		{
			var config = GetValidConfig();

			config.Mongo = null;
			config.PostgreSql = new PostgresConfig
			{
				DatabaseName = "db"
			};

			Assert.Throws<ArgumentException>(() => config.ThrowIfInvalid());
		}

		[Fact]
		public void Postgres_DatabaseName_NotConfigured_Throws()
		{
			var config = GetValidConfig();

			config.Mongo = null;
			config.PostgreSql = new PostgresConfig
			{
				Host = "host"
			};

			Assert.Throws<ArgumentException>(() => config.ThrowIfInvalid());
		}

		[Fact]
		public void Mongo_ConnectionString_NotConfigured_Throws()
		{
			var config = GetValidConfig();

			config.Mongo.ConnectionString = null;

			Assert.Throws<ArgumentException>(() => config.ThrowIfInvalid());
		}

		[Fact]
		public void Mongo_DatabaseName_NotConfigured_Throws()
		{
			var config = GetValidConfig();

			config.Mongo.DatabaseName = null;

			Assert.Throws<ArgumentException>(() => config.ThrowIfInvalid());
		}
	}
}
