﻿using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using R5.FFDB.Components;
using R5.FFDB.Components.CoreData;
using R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1;
//using R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1.Mappers;
using R5.FFDB.Components.CoreData.TeamGames;
using R5.FFDB.Components.CoreData.WeekStats;
using R5.FFDB.Components.Extensions.JsonConverters;
using R5.FFDB.Components.Pipelines.Stats;
using R5.FFDB.Components.PlayerMatcher;
using R5.FFDB.Components.Resolvers;
using R5.FFDB.Components.SourceDataMappers.TeamGames;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.Mongo.DatabaseProvider;
using R5.FFDB.DbProviders.PostgreSql;
using R5.FFDB.DbProviders.PostgreSql.DatabaseProvider;
using R5.FFDB.DbProviders.PostgreSql.Models;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities.WeekStats;
using R5.FFDB.Engine;
using R5.Lib.Pipeline;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DevTester
{
	public class TestContext
	{
		public bool Bool { get; set; }
	}

	public class DevProgram
	{
		private static IServiceProvider _serviceProvider { get; set; }
		private static ILogger<DevProgram> _logger { get; set; }
		private static IDatabaseContext _dbContext { get; set; }
		private static DataDirectoryPath _dataPath { get; set; }

		static DevProgram()
		{
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings
			{
				Formatting = Formatting.Indented,
				Converters = new List<JsonConverter>
				{
					new WeekInfoJsonConverter()
				}
			};
		}

		public static async Task Main(string[] args)
		{
			_serviceProvider = DevTestServiceProvider.Build();
			_logger = _serviceProvider.GetService<ILogger<DevProgram>>();
			var dbProvider = _serviceProvider.GetRequiredService<IDatabaseProvider>();
			_dbContext = dbProvider.GetContext();
			_dataPath = _serviceProvider.GetRequiredService<DataDirectoryPath>();
			/// DONT TOUCH ABOVE ///
			/// 
			/// 

			//RosterSource rostersSource = ActivatorUtilities.CreateInstance<RosterSource>(_serviceProvider,
			//	new R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1.Mappers.ToCoreDataMapper()

			//	);

			//var rostersSource = _serviceProvider.GetRequiredService<IRosterSource>();

			//var teams = TeamDataStore.GetAll();

			//Roster roster = await rostersSource.GetAsync(teams.First());


			var rosterCache = _serviceProvider.GetRequiredService<IRosterCache>();


			// 2552301    2532792
			var task1 = rosterCache.GetPlayerDataAsync("2552301");
			var task2 = rosterCache.GetPlayerDataAsync("2532792");
			var task3 = rosterCache.GetPlayerDataAsync("2555255");
			var task4 = rosterCache.GetPlayerDataAsync("2495136");

			var data = await Task.WhenAll(task1, task2, task3, task4);







			//WeekGameMapSource weekGameMapSource = ActivatorUtilities.CreateInstance<WeekGameMapSource>(_serviceProvider,
			//	new ToVersionedModelMapper(), new ToCoreDataMapper());

			//List<WeekGameMapping> games = await weekGameMapSource.GetAsync(new WeekInfo(2018, 3));




			return;
			Console.ReadKey();
		}

		
















		private static List<WeekGameMatchup> GetMatchups(WeekInfo week)
		{
			var result = new List<WeekGameMatchup>();

			var filePath = _dataPath.Static.WeekGames + $"{week.Season}-{week.Week}.xml";

			XElement weekGameXml = XElement.Load(filePath);

			XElement gameNode = weekGameXml.Elements("gms").Single();

			foreach (XElement game in gameNode.Elements("g"))
			{
				var matchup = new WeekGameMatchup
				{
					Season = week.Season,
					Week = week.Week
				};

				matchup.HomeTeamId = TeamDataStore.GetIdFromAbbreviation(game.Attribute("h").Value, includePriorLookup: true);
				matchup.AwayTeamId = TeamDataStore.GetIdFromAbbreviation(game.Attribute("v").Value, includePriorLookup: true);
				matchup.NflGameId = game.Attribute("eid").Value;
				matchup.GsisGameId = game.Attribute("gsis").Value;

				result.Add(matchup);
			}

			return result;
		}







		private static MongoDbProvider GetMongoDbProvider(ILoggerFactory loggerFactory)
		{
			var config = new MongoConfig
			{
				ConnectionString = "mongodb://localhost:27017/FFDB_Test_1?replicaSet=rs_local",
				DatabaseName = "FFDB_Test_1"
			};

			return new MongoDbProvider(config, loggerFactory);
		}

		private static PostgresDbProvider GetPostgresDbProvider(ILoggerFactory loggerFactory)
		{
			var _config = new PostgresConfig
			{
				DatabaseName = "ffdb_test_1",
				Host = "localhost",
				Username = "ffdb",
				Password = "welc0me!"
			};

			return new PostgresDbProvider(_config, loggerFactory);
		}


		private static NpgsqlConnection GetConnection()
		{
			var _config = new PostgresConfig
			{
				DatabaseName = "ffdb_test_1",
				Host = "localhost",
				Username = "ffdb",
				Password = "welc0me!"
			};

			string connectionString = $"Host={_config.Host};Database={_config.DatabaseName};";

			if (_config.IsSecured)
			{
				connectionString += $"Username={_config.Username};Password={_config.Password}";
			}

			return new NpgsqlConnection(connectionString);
		}

		private static FfdbEngine GetConfiguredPostgresEngine()
		{
			var setup = new EngineSetup();

			setup.UsePostgreSql(new PostgresConfig
			{
				DatabaseName = "ffdb_test_1",
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
				ConnectionString = "mongodb://localhost:27017/FFDB_Test_1?replicaSet=rs_local",
				DatabaseName = "FFDB_Test_1"
			});

			return GetConfiguredEngine(setup);
		}

		private static FfdbEngine GetConfiguredEngine(EngineSetup setup)
		{
			//var setup = new EngineSetup();

			setup
				.SetRootDataDirectoryPath(@"D:\Repos\ffdb_data_2\");
				//.UsePostgreSql(new PostgresConfig
				//{
				//	DatabaseName = "ffdb_test_1",
				//	Host = "localhost",
				//	Username = "ffdb",
				//	Password = "welc0me!"
				//});

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
