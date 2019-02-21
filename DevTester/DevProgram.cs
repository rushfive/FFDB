using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using R5.FFDB.Components;
using R5.FFDB.Components.CoreData;
using R5.FFDB.Components.CoreData.Dynamic.Rosters;
using R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1;
using R5.FFDB.Components.CoreData.Static.PlayerStats.Sources.V1;
using R5.FFDB.Components.CoreData.Static.PlayerStats.Sources.V1.Mappers;
using R5.FFDB.Components.CoreData.Static.WeekMatchups.Sources.V1;
using R5.FFDB.Components.CoreData.Static.WeekMatchups.Sources.V1.Mappers;
using R5.FFDB.Components.Extensions.JsonConverters;
using R5.FFDB.Components.Http;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.Mongo;
using R5.FFDB.DbProviders.PostgreSql;
using R5.FFDB.DbProviders.PostgreSql.DatabaseProvider;
using R5.FFDB.DbProviders.PostgreSql.Models;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities.WeekStats;
using R5.FFDB.Engine;
using R5.Lib.Pipeline;
using R5.PostgresMapper.Models;
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
				Formatting = Formatting.None,
				Converters = new List<JsonConverter>
				{
					new WeekInfoJsonConverter()
				}
			};
		}

		public static async Task Main(string[] args)
		{
			//_serviceProvider = DevTestServiceProvider.Build();
			//_logger = _serviceProvider.GetService<ILogger<DevProgram>>();
			//var dbProvider = _serviceProvider.GetRequiredService<IDatabaseProvider>();
			//_dbContext = dbProvider.GetContext();
			//_dataPath = _serviceProvider.GetRequiredService<DataDirectoryPath>();

			//FfdbEngine engine = GetConfiguredMongoEngine();
			//await engine.RunInitialSetupAsync();

			//var teamType = typeof(TeamSql);
			//string createTable = SqlCommandBuilder.Table.Create(teamType);


			var test1 = new TestEntity();
			var test2 = new TestEntity2();

			var t1Name = test1.TableName;
			var t2Name = test2.TableName;

			var t1Cols = test1.Columns();
			var t1bCols = test1.Columns();
			var t2Cols = test2.Columns();




			return;
			Console.ReadKey();
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
				ConnectionString = "mongodb://localhost:27017/FFDB_Test_2?replicaSet=rs_local",
				DatabaseName = "FFDB_Test_2"
			});

			return GetConfiguredEngine(setup);
		}

		private static FfdbEngine GetConfiguredEngine(EngineSetup setup)
		{
			setup
				.SetRootDataDirectoryPath(@"D:\Repos\ffdb_data_3\");

			setup.WebRequest
				.SetThrottle(3000)
				.AddDefaultBrowserHeaders();

			setup.Logging
				.SetLogDirectory(@"D:\Repos\ffdb_data_3\dev_test_logs\")
				.SetRollingInterval(RollingInterval.Day)
				.SetLogLevel(LogEventLevel.Debug);

			setup
				.SkipRosterFetch()
				.SaveToDisk()
				.SaveOriginalSourceFiles();

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
