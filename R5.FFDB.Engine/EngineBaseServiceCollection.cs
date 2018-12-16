using Microsoft.Extensions.DependencyInjection;
using R5.FFDB.Components;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.CoreData.PlayerProfile;
using R5.FFDB.Components.CoreData.PlayerProfile.Models;
using R5.FFDB.Components.CoreData.Roster;
using R5.FFDB.Components.CoreData.TeamGameHistory;
using R5.FFDB.Components.CoreData.WeekStats;
using R5.FFDB.Components.ErrorFileLog;
using R5.FFDB.Components.Http;
using R5.FFDB.Components.Mappers;
using R5.FFDB.Components.Resolvers;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Database;
using R5.FFDB.Engine.Source;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Engine
{
	public class EngineBaseServiceCollection
	{
		private string _rootDataPath { get; set; }
		private WebRequestConfig _webRequestConfig { get; set; }
		private LoggingConfig _loggingConfig { get; set; }
		private IDatabaseProvider _databaseProvider { get; set; }

		public ServiceCollection Create()
		{
			ValidateConfigurations();

			var services = new ServiceCollection();

			services.AddLogging();
			
			var dataPath = new DataDirectoryPath(_rootDataPath);

			var throttle = new WebRequestThrottle(
				_webRequestConfig.ThrottleMilliseconds,
				_webRequestConfig.RandomizedThrottle);

			services
				.AddScoped(sp => dataPath)
				.AddScoped(sp => _webRequestConfig)
				.AddScoped(sp => throttle)
				.AddScoped<SourcesResolver>()
				.AddScoped<LatestWeekValue>()
				.AddScoped<PlayerProfiles>()
				.AddScoped(sp => _databaseProvider);

			services
				.AddScoped<IWebRequestClient, WebRequestClient>()
				.AddScoped<IPlayerProfileSource, PlayerProfileSource>()
				.AddScoped<IRosterSource, RosterSource>()
				.AddScoped<IWeekStatsSource, WeekStatsSource>()
				.AddScoped<ITeamGameHistorySource, TeamGameHistorySource>()
				.AddScoped<IAvailableWeeksResolver, AvailableWeeksResolver>()
				.AddScoped<IPlayerIdMapper, PlayerIdMapper>()
				.AddScoped<IErrorFileLogger, ErrorFileLogger>();

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
			if (_databaseProvider == null)
			{
				throw new InvalidOperationException("Database provider must be provided.");
			}
		}

		public EngineBaseServiceCollection SetRootDataPath(string path)
		{
			_rootDataPath = path;
			return this;
		}

		public EngineBaseServiceCollection AddWebRequestConfig(WebRequestConfig config)
		{
			_webRequestConfig = config;
			return this;
		}

		public EngineBaseServiceCollection AddLoggingConfig(LoggingConfig config)
		{
			_loggingConfig = config;
			return this;
		}

		public EngineBaseServiceCollection AddDatabaseProvider(IDatabaseProvider provider)
		{
			_databaseProvider = provider;
			return this;
		}
	}

	public static class EngineBaseServiceCollectionExtensions
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
