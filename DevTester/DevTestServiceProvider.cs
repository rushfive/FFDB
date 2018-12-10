using DevTester.Testers;
using Microsoft.Extensions.DependencyInjection;
using R5.FFDB.Components;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.ErrorFileLog;
using R5.FFDB.Components.PlayerProfile;
using R5.FFDB.Components.PlayerProfile.Sources.NFLWeb;
using R5.FFDB.Components.PlayerTeamHistory;
using R5.FFDB.Components.PlayerTeamHistory.Sources.NFLWeb;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevTester
{
	public static class DevTestServiceProvider
	{
		public static IServiceProvider Build()
		{
			var services = new ServiceCollection();

			WebRequestClient webClient = GetWebRequestClient();
			var throttle = new WebRequestThrottle(1000);
			var dataPath = new DataDirectoryPath(@"D:\Repos\ffdb_data\");

			services
				.AddScoped<IWebRequestClient>(sp => webClient)
				.AddScoped<DataDirectoryPath>(sp => dataPath)
				.AddScoped<WebRequestThrottle>(sp => throttle)
				.AddScoped<IPlayerProfileSource, PlayerProfileSource>()
				.AddScoped<IErrorFileLogger, ErrorFileLogger>();

			AddLogging(services, @"D:\Repos\ffdb_data\dev_test_logs\");

			services
				.AddScoped<IPlayerProfileTester, PlayerProfileTester>()
				.AddScoped<IPlayerTeamHistoryTester, PlayerTeamHistoryTester>();

			services
				.AddScoped<IPlayerTeamHistorySource, PlayerTeamHistorySource>();

			return services.BuildServiceProvider();

			// local funcs
			void AddLogging(ServiceCollection sc, string logDirectory)
			{
				Log.Logger = new LoggerConfiguration()
					.MinimumLevel.Debug()
					.WriteTo.Console()
					.WriteTo.File(
						logDirectory + ".txt",
						fileSizeLimitBytes: null,
						restrictedToMinimumLevel: LogEventLevel.Debug,
						rollingInterval: RollingInterval.Day,
						rollOnFileSizeLimit: false)
					.CreateLogger();

				sc.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
			}
		}

		public static WebRequestClient GetWebRequestClient()
		{
			var headers = new Dictionary<string, string>
			{
				//{ "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8" },
				//{ "Accept-Encoding", "gzip, deflate" },
				//{ "Accept-Language", "en-US,en;q=0.9" },
				{ "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36" }
			};

			var config = new WebRequestConfig(0, (3000, 8000), headers);
			return new WebRequestClient(config);
		}
	}
}
