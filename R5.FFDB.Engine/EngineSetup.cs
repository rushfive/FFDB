using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Core.Database;
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
		private ProgramOptions _programOptions { get; } = new ProgramOptions();

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
			if (string.IsNullOrWhiteSpace(config.ConnectionString))
			{
				throw new ArgumentException("Mongo connection string must be provided in the config.");
			}
			if (string.IsNullOrWhiteSpace(config.DatabaseName))
			{
				throw new ArgumentException("Mongo database name must be provided in the config.");
			}

			_mongoConfig = config;
			return this;
		}

		public EngineSetup SkipRosterFetch()
		{
			_programOptions.SkipRosterFetch = true;
			return this;
		}

		public EngineSetup SaveToDisk()
		{
			_programOptions.SaveToDisk = true;
			return this;
		}

		public EngineSetup SaveOriginalSourceFiles()
		{
			_programOptions.SaveOriginalSourceFiles = true;
			return this;
		}

		public FfdbEngine Create()
		{
			var baseServiceCollection = new EngineBaseServiceCollection();
			SetDatabaseProviderFactory(baseServiceCollection);

			ServiceCollection services = baseServiceCollection
				.SetRootDataPath(_rootDataPath)
				.SetWebRequestConfig(WebRequest.Build())
				.SetLoggingConfig(Logging.Build())
				.SetProgramOptions(_programOptions)
				.Create();
			
			return services
				.AddScoped<FfdbEngine>()
				.BuildServiceProvider()
				.GetService<FfdbEngine>();
		}

		private void SetDatabaseProviderFactory(EngineBaseServiceCollection collection)
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
			// TODO: Configuring the engine with a datbase config should create this 
			// dbProviderFactory on the spot, so we dont even need to have this method.
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

			collection.SetDatabaseProviderFactory(dbProviderFactory);
		}
	}
}
