using Microsoft.Extensions.DependencyInjection;
using R5.FFDB.Components;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
using R5.FFDB.DbProviders.Mongo;
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
		private Func<IAppLogger, IDatabaseProvider> _dbProviderFactory { get; set; }
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
				Directory.CreateDirectory(path);
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

			_dbProviderFactory = logger => new PostgresDbProvider(config, logger);
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

			_dbProviderFactory = logger => new MongoDbProvider(config, logger);
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

		public EngineSetup EnableFetchingFromDataRepo()
		{
			_programOptions.DataRepoEnabled = true;
			return this;
		}

		public FfdbEngine Create()
		{
			var baseServiceCollection = new EngineBaseServiceCollection();

			ServiceCollection services = baseServiceCollection
				.SetRootDataPath(_rootDataPath)
				.SetDatabaseProviderFactory(_dbProviderFactory)
				.SetWebRequestConfig(WebRequest.Build())
				.SetLoggingConfig(Logging.Build())
				.SetProgramOptions(_programOptions)
				.Create();
			
			return services
				.AddScoped<FfdbEngine>()
				.BuildServiceProvider()
				.GetService<FfdbEngine>();
		}
	}
}
