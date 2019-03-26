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
	/// <summary>
	/// Builder class used to help build out and configure the FfdbEngine.
	/// </summary>
	public class EngineSetup
	{
		/// <summary>
		/// Builder for the web request client used by the engine.
		/// </summary>
		public WebRequestConfigBuilder WebRequest { get; } = WebRequestConfigBuilder.WithDefaultBrowserHeaders();

		/// <summary>
		/// Builder for logging configuration.
		/// </summary>
		public LoggingConfigBuilder Logging { get; } = new LoggingConfigBuilder();

		private string _rootDataPath { get; set; }
		private Func<IAppLogger, IDatabaseProvider> _dbProviderFactory { get; set; }
		private ProgramOptions _programOptions { get; } = new ProgramOptions();

		/// <summary>
		/// Sets the data directory path where data files are persisted into.
		/// </summary>
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

		/// <summary>
		/// Sets the Engine to use PostgreSql as its datastore.
		/// </summary>
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

		/// <summary>
		/// Sets the Engine to use Mongo as its datastore.
		/// </summary>
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

		/// <summary>
		/// Sets the Engine to use your own custom database.
		/// </summary>
		/// <param name="dbProviderFactory">Database provider factory function.</param>
		public EngineSetup UseCustomDbProvider(Func<IAppLogger, IDatabaseProvider> dbProviderFactory)
		{
			_dbProviderFactory = dbProviderFactory;
			return this;
		}

		/// <summary>
		/// Skips the fetching of team roster information.
		/// </summary>
		public EngineSetup SkipRosterFetch()
		{
			_programOptions.SkipRosterFetch = true;
			return this;
		}

		/// <summary>
		/// Configures the Engine to save the versioned models to disk.
		/// </summary>
		public EngineSetup SaveToDisk()
		{
			_programOptions.SaveToDisk = true;
			return this;
		}

		/// <summary>
		/// Configures the Engine to save the original sources data files to disk.
		/// </summary>
		/// <returns></returns>
		public EngineSetup SaveOriginalSourceFiles()
		{
			_programOptions.SaveOriginalSourceFiles = true;
			return this;
		}

		/// <summary>
		/// Enables the fetching of versioned data from the data repository.
		/// </summary>
		public EngineSetup EnableFetchingFromDataRepo()
		{
			_programOptions.DataRepoEnabled = true;
			return this;
		}

		/// <summary>
		/// Create an instance of the FfdbEngine based on this builder's configuration.
		/// </summary>
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
