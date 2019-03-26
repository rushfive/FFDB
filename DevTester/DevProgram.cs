using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
using R5.FFDB.Components;
using R5.FFDB.Components.Extensions.JsonConverters;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.Mongo;
using R5.FFDB.DbProviders.PostgreSql.DatabaseProvider;
using R5.FFDB.Engine;
using R5.Internals.PostgresMapper.Models;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using R5.Internals.PostgresMapper;
using System.Linq.Expressions;
using System.Text;
using R5.Internals.PostgresMapper.SqlBuilders;
using R5.Internals.Abstractions.Expressions;
using R5.Internals.PostgresMapper.Attributes;
using R5.Internals.PostgresMapper.QueryCommand;
using R5.FFDB.DbProviders.PostgreSql.Entities.WeekStats;
using R5.FFDB.DbProviders.PostgreSql.Entities;
using Serilog.Context;
using R5.FFDB.Core;

namespace DevTester
{

	public class DevProgram
	{
		private static IServiceProvider _serviceProvider { get; } = DevTestServiceProvider.Build();
		private static IAppLogger _logger { get; set; }
		private static IDatabaseContext _dbContext { get; set; }
		private static DataDirectoryPath _dataPath { get; set; }

		static DevProgram()
		{
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings
			{
				Formatting = Formatting.None,
				Converters = new List<JsonConverter>
				{
					new WeekInfoJsonConverter()
				}
			};
		}

		public static async Task Main(string[] args)
		{

			FfdbEngine engine = GetConfiguredPostgresEngine();

			//await engine.RunInitialSetupAsync(skipAddingStats: true);
			//bool isInit = await engine.HasBeenInitializedAsync();

			await engine.RunInitialSetupAsync(skipAddingStats: false);

			//await engine.Stats.AddForWeekAsync(new WeekInfo(2018, 2));

			//await engine.RunInitialSetupAsync(skipAddingStats: true);

			//await engine.Team.UpdateRosterMappingsAsync();
			//await engine.Stats.AddMissingAsync();

			//await engine.Stats.AddForWeekAsync(new WeekInfo(2018, 17));
			//List<WeekInfo> updatedWeeks = await engine.GetAllUpdatedWeeksAsync();
			

			return;
			Console.ReadKey();
		}


		private static FfdbEngine GetConfiguredPostgresEngine()
		{
			var setup = new EngineSetup();

			setup.UsePostgreSql(new PostgresConfig
			{
				DatabaseName = "ffdb_test_5",
				Host = "localhost",
				Username = "ffdb",
				Password = "welc0me!"
			});

			return GetConfiguredEngine(setup);
		}

		private static FfdbEngine GetConfiguredMongoEngine()
		{
			var setup = new EngineSetup();

			setup.UseMongo(new MongoConfig
			{
				ConnectionString = "mongodb://localhost:27017/FFDB_Test_2?replicaSet=rs_local",
				DatabaseName = "FFDB_Test_3"
			});

			return GetConfiguredEngine(setup);
		}

		private static FfdbEngine FOR_DOCS()
		{
var setup = new EngineSetup();

setup
	.SetRootDataDirectoryPath(@"C:\path\to\data\dir\")
	.UseMongo(new MongoConfig
	{
		ConnectionString = "connection_string",
		DatabaseName = "db_name"
	});

FfdbEngine engine = setup.Create();

			return GetConfiguredEngine(setup);
		}

		private static FfdbEngine GetConfiguredEngine(EngineSetup setup)
		{
			setup
				.SetRootDataDirectoryPath(@"D:\Repos\ffdb_data_5\");
				//.SetRootDataDirectoryPath(@"D:\Repos\FFDB.Data\");

			setup.WebRequest
				.SetThrottle(3000)
				.AddDefaultBrowserHeaders();

			setup.Logging
				.SetLogDirectory(@"D:\Repos\ffdb_logs\")
				.SetRollingInterval(RollingInterval.Day)
				.UseDebugLogLevel();

			setup
				.SkipRosterFetch()
				.SaveToDisk()
				.SaveOriginalSourceFiles()
				.EnableFetchingFromDataRepo();

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
