using Microsoft.Extensions.DependencyInjection;
using R5.FFDB.Components;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.ErrorFileLog;
using R5.FFDB.Components.PlayerProfile.Sources.NFLWeb;
using R5.FFDB.Components.Roster.Sources.NFLWebTeam;
using R5.FFDB.Components.WeekStats.Sources.NFLFantasyApi;
using R5.FFDB.Engine.ConfigBuilders;
using R5.FFDB.Engine.Source;
using R5.FFDB.Engine.Source.Resolvers;
using Serilog;
using System;
using System.IO;

namespace R5.FFDB.Engine
{
	public class EngineSetup
	{
		public WebRequestConfigBuilder WebRequest { get; } = new WebRequestConfigBuilder();
		//public FileDownloadConfigBuilder FileDownload { get; } = new FileDownloadConfigBuilder();
		public LoggingConfigBuilder Logging { get; } = new LoggingConfigBuilder();

		private string _rootDataPath { get; set; }

		public void SetRootDataDirectoryPath(string path)
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
		}

		public FfdbEngine Create()
		{
			if (string.IsNullOrWhiteSpace(_rootDataPath))
			{
				throw new InvalidOperationException("Root data directory path must be provided.");
			}
			var dataPath = new DataDirectoryPath(_rootDataPath);

			WebRequestConfig webRequestConfig = WebRequest.Build();
			var throttle = new WebRequestThrottle(webRequestConfig.ThrottleMilliseconds, webRequestConfig.RandomizedThrottle);

			LoggingConfig loggingConfig = Logging.Build();
			
			var services = new ServiceCollection();

			services
				.AddScoped(sp => webRequestConfig)
				.AddScoped(sp => dataPath)
				.AddScoped(sp => throttle)
				.AddScoped<IWebRequestClient, WebRequestClient>()

				//.AddScoped<IPlayerDataSource, PlayerDataSource>()
				//.AddScoped<IRosterSource, RosterSource>()
				//.AddScoped<IWeekStatsSource, WeekStatsSource>()
				//.AddScoped<IDepthChartSource, DepthChartSource>()

				// scoped or singleton?? being served from a singleton provider
				.AddScoped<PlayerProfileSource>()
				.AddScoped<RosterSource>()
				.AddScoped<WeekStatsSource>()

				.AddScoped<IPlayerDataSourceResolver, PlayerDataSourceResolver>()
				.AddScoped<IRosterSourceResolver, RosterSourceResolver>()
				.AddScoped<IWeekStatsSourceResolver, WeekStatsSourceResolver>()
				
				.AddScoped<ISourcesFactory, SourcesFactory>()

				.AddLogging(loggingConfig)
				.AddScoped<IErrorFileLogger, ErrorFileLogger>()
				.AddScoped<FfdbEngine>();

			return services
				.BuildServiceProvider()
				.GetService<FfdbEngine>();
		}
	}

	public static class EngineSetupExtensions
	{
		public static IServiceCollection AddLogging(this IServiceCollection services, LoggingConfig config)
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
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
