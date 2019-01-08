using DevTester.Testers;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using R5.FFDB.Components;
using R5.FFDB.Components.CoreData.PlayerProfile;
using R5.FFDB.Components.CoreData.PlayerProfile.Models;
using R5.FFDB.Components.CoreData.Roster;
using R5.FFDB.Components.CoreData.Roster.Values;
using R5.FFDB.Components.CoreData.TeamData.Models;
using R5.FFDB.Components.CoreData.TeamGameHistory;
using R5.FFDB.Components.CoreData.WeekStats;
using R5.FFDB.Components.Resolvers;
using R5.FFDB.Core.Models;
using R5.FFDB.Database;
using R5.FFDB.Database.DbContext;
using R5.FFDB.DbProviders.PostgreSql;
using R5.FFDB.DbProviders.PostgreSql.DatabaseProvider;
using R5.FFDB.DbProviders.PostgreSql.Models;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities.WeekStats;
using R5.FFDB.Engine;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DevTester
{
	public class DevProgram
	{
		private static IServiceProvider _serviceProvider { get; set; }
		private static ILogger<DevProgram> _logger { get; set; }
		private static IDatabaseContext _dbContext { get; set; }

		public static async Task Main(string[] args)
		{
			_serviceProvider = DevTestServiceProvider.Build();
			_logger = _serviceProvider.GetService<ILogger<DevProgram>>();
			var dbProvider = _serviceProvider.GetRequiredService<IDatabaseProvider>();
			_dbContext = dbProvider.GetContext();
			/// DONT TOUCH ABOVE ///
			/// 
			
			
			FfdbEngine engine = GetConfiguredEngine();
			//await engine.RunInitialSetupAsync();
			//await engine.Update.UpdateRostersAsync();
			//await engine.Update.UpdateStatsForWeekAsync(new WeekInfo(2010, 2));
			//await engine.Stats.AddMissingAsync();
			//await engine.CheckSourcesHealthAsync();

			//await engine.Player.UpdateCurrentlyRosteredAsync();

			//await engine.Stats.RemoveForWeekAsync(new WeekInfo(2018, 5));
			//await engine.Stats.RemoveAllAsync();
			await engine.Stats.AddMissingAsync();

			return;
			Console.ReadKey();
		}
		
		
		private static FfdbEngine GetConfiguredEngine()
		{
			var setup = new EngineSetup();

			setup
				.SetRootDataDirectoryPath(@"D:\Repos\ffdb_data_2\")
				.UsePostgreSql(new PostgresConfig
				{
					DatabaseName = "ffdb_test_1",
					Host = "localhost",
					Username = "ffdb",
					Password = "welc0me!"
				});

			setup.WebRequest
				.SetThrottle(1000)
				.AddDefaultBrowserHeaders();

			setup.Logging
				.SetLogDirectory(@"D:\Repos\ffdb_data_2\dev_test_logs\")
				.SetRollingInterval(RollingInterval.Day)
				.SetLogLevel(LogEventLevel.Debug);

			return setup.Create();
		}
		

		private static async Task<HtmlDocument> DownloadPageAsync(
			string pageUri, string filePath, bool skipFetch)
		{
			//string uri = "http://www.nfl.com/player/mikemitchell/238227/gamelogs?season=2018";
			//string filePath = @"D:\Repos\ffdb_data\temp\debug.html";

			if (!skipFetch)
			{
				var web = new HtmlWeb();
				HtmlDocument doc = await web.LoadFromWebAsync(pageUri);
				doc.Save(filePath);
			}
			
			string html = File.ReadAllText(filePath);
			var page = new HtmlDocument();
			page.LoadHtml(html);

			return page;
		}
	}
}
