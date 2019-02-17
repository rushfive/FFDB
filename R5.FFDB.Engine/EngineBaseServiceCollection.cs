using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using R5.FFDB.Components;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.CoreData;
using R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1;
using R5.FFDB.Components.Http;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core.Database;
using Serilog;
using Serilog.Events;
using System;

namespace R5.FFDB.Engine
{
	public class EngineBaseServiceCollection
	{
		private ServiceCollection _services { get; } = new ServiceCollection();

		private string _rootDataPath { get; set; }
		private WebRequestConfig _webRequestConfig { get; set; }
		private LoggingConfig _loggingConfig { get; set; }
		private Func<ILoggerFactory, IDatabaseProvider> _dbProviderFactory { get; set; }
		private ProgramOptions _programOptions { get; set; }

		public ServiceCollection Create()
		{
			ValidateConfigurations();

			var services = new ServiceCollection();

			services.AddLogging(_loggingConfig);
			
			var dataPath = new DataDirectoryPath(_rootDataPath);

			var throttle = new WebRequestThrottle(
				_webRequestConfig.ThrottleMilliseconds,
				_webRequestConfig.RandomizedThrottle);

			var programOptions = _programOptions ?? new ProgramOptions();

			services
				.AddScoped(sp => dataPath)
				.AddScoped(sp => _webRequestConfig)
				.AddScoped(sp => throttle)
				.AddScoped<LatestWeekValue>()
				.AddScoped<AvailableWeeksValue>()
				//.AddScoped<RostersValue>()
				.AddScoped<ProgramOptions>(sp => programOptions)
				.AddScoped<IDatabaseProvider>(sp =>
				{
					var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
					return _dbProviderFactory(loggerFactory);
				});

			services
				.AddScoped<IWebRequestClient, WebRequestClient>();
			//.AddScoped<IPlayerSource, PlayerSource>()
			//.AddScoped<IPlayerService, PlayerService>()
			//.AddScoped<IPlayerScraper, PlayerScraper>()
			//.AddScoped<IRosterSource, RosterSource>()
			//.AddScoped<IRosterScraper, RosterScraper>()
			//.AddScoped<IWeekStatsSource, WeekStatsSource>()
			//.AddScoped<IWeekStatsService, WeekStatsService>()
			//.AddScoped<ITeamGamesSource, TeamGamesSource>()
			//.AddScoped<ITeamGameStatsService, TeamGameStatsService>()
			//.AddScoped<IWeekGameMatchupService, WeekGameMatchupService>()
			//.AddScoped<IProcessorHelper, ProcessorHelper>()
			//.AddScoped<IPlayerMatcherFactory, PlayerMatcherFactory>()
			//.AddScoped<ITeamGamesDataMapper, TeamGamesDataMapper>()
			//.AddScoped<ITeamGameDataCache, TeamGameDataCache>()
			//.AddScoped<IWeekGameDataCache, WeekGameDataCache>()
			//.AddScoped<IPlayerWeekTeamResolverFactory, PlayerWeekTeamResolverFactory>();

			// NEW:

			services.AddAsyncLazyCache();

			// for RosterSource
			services
				.AddCoreDataSources();

			return services;
		}

		private void ValidateConfigurations()
		{
			if (string.IsNullOrWhiteSpace(_rootDataPath))
			{
				throw new InvalidOperationException("Root data directory path must be provided.");
			}
			if (_webRequestConfig == null)
			{
				throw new InvalidOperationException("Web request config must be provided.");
			}
			if (_loggingConfig == null)
			{
				throw new InvalidOperationException("Logging config must be provided.");
			}
			if (_dbProviderFactory == null)
			{
				throw new InvalidOperationException("Database provider factory must be provided.");
			}
		}

		public EngineBaseServiceCollection SetRootDataPath(string path)
		{
			_rootDataPath = path;
			return this;
		}

		public EngineBaseServiceCollection SetWebRequestConfig(WebRequestConfig config)
		{
			_webRequestConfig = config;
			return this;
		}

		public EngineBaseServiceCollection SetLoggingConfig(LoggingConfig config)
		{
			_loggingConfig = config;
			return this;
		}

		public EngineBaseServiceCollection SetDatabaseProviderFactory(
			Func<ILoggerFactory, IDatabaseProvider> dbProviderFactory)
		{
			_dbProviderFactory = dbProviderFactory;
			return this;
		}

		public EngineBaseServiceCollection SetProgramOptions(ProgramOptions options)
		{
			_programOptions = options;
			return this;
		}
	}

	public static class EngineBaseServiceCollectionExtensions
	{
		public static IServiceCollection AddLogging(this IServiceCollection services, LoggingConfig config)
		{
			var loggerConfig = new LoggerConfiguration();
			switch (config.LogLevel)
			{
				case LogEventLevel.Verbose:
					loggerConfig = loggerConfig.MinimumLevel.Verbose();
					break;
				case LogEventLevel.Debug:
					loggerConfig = loggerConfig.MinimumLevel.Debug();
					break;
				case LogEventLevel.Information:
					loggerConfig = loggerConfig.MinimumLevel.Information();
					break;
				case LogEventLevel.Warning:
					loggerConfig = loggerConfig.MinimumLevel.Warning();
					break;
				case LogEventLevel.Error:
					loggerConfig = loggerConfig.MinimumLevel.Error();
					break;
				case LogEventLevel.Fatal:
					loggerConfig = loggerConfig.MinimumLevel.Fatal();
					break;
				default:
					throw new ArgumentOutOfRangeException($"'{config.LogLevel}' is an invalid serilog log event level.");
			}
			
			Log.Logger = loggerConfig
				.WriteTo.Console()
				.WriteTo.File(
					config.LogDirectory + ".txt",
					fileSizeLimitBytes: config.MaxBytes,
					restrictedToMinimumLevel: config.LogLevel,
					rollingInterval: config.RollingInterval,
					rollOnFileSizeLimit: config.RollOnFileSizeLimit)
				.CreateLogger();
			
			return services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
		}
	}
}
