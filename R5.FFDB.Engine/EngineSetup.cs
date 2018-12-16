using Microsoft.Extensions.DependencyInjection;
using R5.FFDB.Database;
using R5.FFDB.DbProviders.PostgreSql;
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
		private IDatabaseProvider _databaseProvider { get; set; }

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
			// todo: validate config

			_databaseProvider = new PostgresDbProvider(config);
			return this;
		}

		public EngineSetup UseMongo()
		{
			return this;
		}

		public EngineSetup UseDatabase(IDatabaseProvider provider)
		{
			if (provider == null)
			{
				throw new ArgumentNullException(nameof(provider), "A valid provider must be given.");
			}

			_databaseProvider = provider;
			return this;
		}

		public FfdbEngine Create()
		{
			var baseServiceCollection = new EngineBaseServiceCollection();

			ServiceCollection services = baseServiceCollection
				.SetRootDataPath(_rootDataPath)
				.AddWebRequestConfig(WebRequest.Build())
				.AddLoggingConfig(Logging.Build())
				.AddDatabaseProvider(_databaseProvider)
				.Create();
			
			return services
				.AddScoped<FfdbEngine>()
				.BuildServiceProvider()
				.GetService<FfdbEngine>();
		}
	}
}
