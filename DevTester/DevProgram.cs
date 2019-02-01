using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using R5.FFDB.Components;
using R5.FFDB.Components.CoreData;
using R5.FFDB.Components.CoreData.TeamGames;
using R5.FFDB.Components.CoreData.WeekStats;
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
using R5.Lib.Pipeline.Linked;
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

			var endChain = new LinkedPointerStage<TestContext>("End Stage 1", async context =>
			{
				Console.WriteLine($"[End] Stage 1 - {context.Bool}");
				return ProcessResult.Continue;
			});
			endChain
				.SetNext("End Stage 2", async context =>
				{
					Console.WriteLine($"[End] Stage 2 - {context.Bool}");
					//Console.WriteLine($"[End] Stage 2 - Setting Bool to FALSE");
					//context.Bool = false;
					return ProcessResult.Continue;
				})
				.SetNext(new LinkedPointerStage<TestContext>("terminate stage - no process func"))
				.SetNext("End Stage 3", async context =>
				{
					Console.WriteLine($"[End] Stage 2 - {context.Bool}");
					Console.WriteLine($"[End] Stage 2 - will END processing if FALSE");
					return context.Bool ? ProcessResult.Continue : ProcessResult.End;
				})
				.SetNext("End Stage 4", async context =>
				{
					Console.WriteLine($"[End] Stage 3 - {context.Bool}");
					Console.WriteLine($"[End] Stage 3 - Should have only reached here if context.Bool was TRUE");
					return ProcessResult.End;
				});

			var midChain1 = new LinkedPointerStage<TestContext>("Mid-1 Stage 1", async context =>
			{
				Console.WriteLine($"[Mid-1] Stage 1 - {context.Bool}");
				Console.WriteLine($"[Mid-1] Stage 1 - Reached MID-1 because context.Bool is TRUE");
				Console.WriteLine($"[Mid-1] Stage 1 - Moving onto [End]");
				return ProcessResult.Continue;
			});
			midChain1
				.SetNext(endChain);

			var midChain2 = new LinkedPointerStage<TestContext>("Mid-2 Stage 1", async context =>
			{
				Console.WriteLine($"[Mid-2] Stage 1 - {context.Bool}");
				Console.WriteLine($"[Mid-2] Stage 1 - Reached MID-2 because context.Bool is FALSE");
				return ProcessResult.Continue;
			});
			midChain2
				.SetNext("Mid-2 Stage 2", async context =>
				{
					Console.WriteLine($"[Mid-2] Stage 2 - {context.Bool}");
					Console.WriteLine($"[Mid-2] Stage 2 - Moving onto [End]");
					return ProcessResult.Continue;
				})
				.SetNext(endChain);

			var splitByBool = new LinkedCallbackStage<TestContext>("Begin Stage 2", async context =>
			{
				Console.WriteLine($"[Begin] Stage 2 - {context.Bool} - CALLBACK stage");
				return ProcessResult.Continue;
			})
				.SetCallback(context =>
				{
					return context.Bool ? midChain1 : midChain2;
				});

			var beginChain = new LinkedPointerStage<TestContext>("Begin Stage 1", async context =>
			{
				Console.WriteLine($"[Begin] Stage 1 - {context.Bool}");
				return ProcessResult.Continue;
			});
			beginChain
				.SetNext(splitByBool);


			var pipeline = new LinkedPipeline<TestContext>("Test Pipeline", beginChain);
			await pipeline.ProcessAsync(new TestContext { Bool = true }); // should context be passed in as process arg??







			return;


			// TEST getting targets data
			// first need to come up with fuzzy strin gmatching, use edit distance
			//  - normalize pattern vs search-text by lowercasing all, then run algorithm

			//var filePath = @"D:\Repos\ffdb_data_2\team_game_history\game_stats2\2018123015.json";
			//WeekGameTeamData json = JsonConvert.DeserializeObject<WeekGameTeamData>(File.ReadAllText(filePath));
			

			var dataMApper = _serviceProvider.GetRequiredService<ITeamGamesDataMapper>();
			await dataMApper.Test4();


			return;

			var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
			PostgresDbProvider db2Provider = GetPostgresDbProvider(loggerFactory);
			IDatabaseContext dbContext = db2Provider.GetContext();

			List<Player> players = await dbContext.Player.GetByTeamForWeekAsync(30, new WeekInfo(2018, 15));

			var mongoProvider = GetMongoDbProvider(loggerFactory);
			var mongoContext = mongoProvider.GetContext();

			var mongoPlayers = await mongoContext.Player.GetByTeamForWeekAsync(30, new WeekInfo(2018, 15));



			//var matcher = await playerMatcherFactory.GetAsync(30, new WeekInfo(2018, 15));

			//string tyLock = "Tyler Lockett";
			//Guid id = matcher(tyLock);


			return;

			//var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
			//MongoDbProvider mongoProvider = GetMongoDbProvider(loggerFactory);
			//IDatabaseContext mongoContext = mongoProvider.GetContext();

			//List<WeekInfo> logs = await mongoContext.Log.GetUpdatedWeeksAsync();

			//var matchups = GetMatchups(new WeekInfo(2018, 17));
			

			//FfdbEngine engine = GetConfiguredMongoEngine();
			//await engine.Stats.AddForWeekAsync(new WeekInfo(2018, 5));
			//await engine.Team.UpdateRostersAsync();
			//await engine.Stats.RemoveForWeekAsync(new WeekInfo(2018, 5));
			//await engine.RunInitialSetupAsync(forceReinitialize: false);

			//await engine.Stats.AddMissingAsync();



			return;
			Console.ReadKey();
		}

		
















		private static List<WeekGameMatchup> GetMatchups(WeekInfo week)
		{
			var result = new List<WeekGameMatchup>();

			var filePath = _dataPath.Static.TeamGameHistoryWeekGames + $"{week.Season}-{week.Week}.xml";

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
