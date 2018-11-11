using Microsoft.Extensions.DependencyInjection;
using R5.FFDB.Components;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.PlayerData;
using R5.FFDB.Components.Roster;
using R5.FFDB.Components.WeekStats;
using R5.FFDB.Engine.ConfigBuilders;
using Serilog;

namespace R5.FFDB.Engine
{
	public class EngineSetup
	{
		public WebRequestConfigBuilder WebRequest { get; } = new WebRequestConfigBuilder();
		public FileDownloadConfigBuilder FileDownload { get; } = new FileDownloadConfigBuilder();
		public LoggingConfigBuilder Logging { get; } = new LoggingConfigBuilder();

		public FfdbEngine Create()
		{
			WebRequestConfig webRequestConfig = WebRequest.Build();
			FileDownloadConfig fileDownloadConfig = FileDownload.Build();
			LoggingConfig loggingConfig = Logging.Build();

			var services = new ServiceCollection();

			services
				.AddScoped(sp => webRequestConfig)
				.AddScoped(sp => fileDownloadConfig)
				.AddScoped<IWebRequestClient, WebRequestClient>()
				.AddScoped<IPlayerDataService, PlayerDataService>()
				.AddScoped<IRosterService, RosterService>()
				.AddScoped<IWeekStatsService, WeekStatsService>()
				.AddLogging(loggingConfig)
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
