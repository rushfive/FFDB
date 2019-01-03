using Microsoft.Extensions.DependencyInjection;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.Http;
using R5.FFDB.Database;
using R5.FFDB.DbProviders.PostgreSql;
using R5.FFDB.DbProviders.PostgreSql.DatabaseProvider;
using R5.FFDB.Engine;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevTester
{
	public static class DevTestServiceProvider
	{
		public static IServiceProvider Build()
		{
			var webRequestHeaders = new Dictionary<string, string>
			{
				{ "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36" }
			};
			var webRequestConfig = new WebRequestConfig(1000, null, webRequestHeaders);

			var loggingConfig = new LoggingConfig(
				@"D:\Repos\ffdb_data_2\dev_test_logs\",
				maxBytes: null,
				RollingInterval.Day,
				rollOnFileSizeLimit: false,
				LogEventLevel.Debug);
			
			var postgresConfig = new PostgresConfig
			{
				DatabaseName = "ffdb_test_1",
				Host = "localhost",
				Username = "ffdb",
				Password = "welc0me!"
			};

			var baseServiceCollection = new EngineBaseServiceCollection();

			ServiceCollection services = baseServiceCollection
				.SetRootDataPath(@"D:\Repos\ffdb_data_2\")
				.AddWebRequestConfig(webRequestConfig)
				.AddLoggingConfig(loggingConfig)
				.AddDatabaseProviderFactory(loggerFactory => new PostgresDbProvider(postgresConfig, loggerFactory))
				.Create();

			return services.BuildServiceProvider();
		}
	}
}
