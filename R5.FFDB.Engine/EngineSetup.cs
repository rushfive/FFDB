using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using R5.FFDB.Database;
using R5.FFDB.DbProviders.Mongo.DatabaseProvider;
using R5.FFDB.DbProviders.PostgreSql;
using R5.FFDB.DbProviders.PostgreSql.DatabaseProvider;
using R5.FFDB.Engine.ConfigBuilders;
using System;
using System.Diagnostics;
using System.IO;

namespace R5.FFDB.Engine
{
	public class EngineSetup
	{
		public WebRequestConfigBuilder WebRequest { get; } = new WebRequestConfigBuilder();
		public LoggingConfigBuilder Logging { get; } = new LoggingConfigBuilder();

		private string _rootDataPath { get; set; }

		private PostgresConfig _postgresConfig { get; set; }
		private MongoConfig _mongoConfig { get; set; }

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

		public EngineSetup UseMongo(MongoConfig config)
		{
			// todo: validations
			_mongoConfig = config;
			return this;
		}

		public FfdbEngine Create()
		{
			var baseServiceCollection = new EngineBaseServiceCollection();
			AddDatabaseProviderFactory(baseServiceCollection);

			ServiceCollection services = baseServiceCollection
				.SetRootDataPath(_rootDataPath)
				.AddWebRequestConfig(WebRequest.Build())
				.AddLoggingConfig(Logging.Build())
				//.AddDatabaseProviderFactory(loggerFactory => new PostgresDbProvider(_postgresConfig, loggerFactory))
				.Create();
			
			return services
				.AddScoped<FfdbEngine>()
				.BuildServiceProvider()
				.GetService<FfdbEngine>();
		}

		private void AddDatabaseProviderFactory(EngineBaseServiceCollection collection)
		{
			bool noneConfigured = _postgresConfig == null && _mongoConfig == null;
			if (noneConfigured)
			{
				throw new InvalidOperationException("A database type must be configured.");
			}

			bool moreThanOneConfigured = _postgresConfig != null && _mongoConfig != null;
			if (moreThanOneConfigured)
			{
				throw new InvalidOperationException("Engine can only be configured to use a single database type.");
			}
			
			Func<ILoggerFactory, IDatabaseProvider> dbProviderFactory = null;
			if (_postgresConfig != null)
			{
				dbProviderFactory = loggerFactory => new PostgresDbProvider(_postgresConfig, loggerFactory);
			}
			if (_mongoConfig != null)
			{
				dbProviderFactory = loggerFactory => new MongoDbProvider(_mongoConfig, loggerFactory);
			}

			Debug.Assert(dbProviderFactory != null);

			collection.AddDatabaseProviderFactory(dbProviderFactory);
		}
	}
}
