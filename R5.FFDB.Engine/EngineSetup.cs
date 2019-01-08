using Microsoft.Extensions.DependencyInjection;
using R5.FFDB.Database;
using R5.FFDB.DbProviders.PostgreSql;
using R5.FFDB.DbProviders.PostgreSql.DatabaseProvider;
using R5.FFDB.Engine.ConfigBuilders;
using System;
using System.IO;

namespace R5.FFDB.Engine
{
	public class EngineSetup
	{
		public WebRequestConfigBuilder WebRequest { get; } = new WebRequestConfigBuilder();
		public LoggingConfigBuilder Logging { get; } = new LoggingConfigBuilder();

		private string _rootDataPath { get; set; }
		private PostgresConfig _postgresConfig { get; set; }

		public EngineSetup SetRootDataDirectoryPath(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentNullException(nameof(path), "Root data directory path must be provided.");
			}
			if (!path.EndsWith("\\"))
			{
				path += "\\";
			}
			if (!Directory.Exists(path))
			{
				throw new ArgumentException($"Directory path '{path}' doesn't exist.");
			}

			_rootDataPath = path;
			return this;
		}

		public EngineSetup UsePostgreSql(PostgresConfig config)
		{
			if (string.IsNullOrEmpty(config.Host))
			{
				throw new ArgumentException("PostgreSql hostname must be provided in the config.");
			}
			if (string.IsNullOrEmpty(config.DatabaseName))
			{
				throw new ArgumentException("PostgreSql database name must be provided in the config.");
			}

			_postgresConfig = config;
			return this;
		}

		public EngineSetup UseMongo()
		{
			return this;
		}

		public FfdbEngine Create()
		{
			var baseServiceCollection = new EngineBaseServiceCollection();

			ServiceCollection services = baseServiceCollection
				.SetRootDataPath(_rootDataPath)
				.AddWebRequestConfig(WebRequest.Build())
				.AddLoggingConfig(Logging.Build())
				.AddDatabaseProviderFactory(loggerFactory => new PostgresDbProvider(_postgresConfig, loggerFactory))
				.Create();
			
			return services
				.AddScoped<FfdbEngine>()
				.BuildServiceProvider()
				.GetService<FfdbEngine>();
		}
	}
}
