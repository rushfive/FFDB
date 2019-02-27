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
using R5.Lib.Pipeline;
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
using R5.Internals.PostgresMapper.Query;

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


			//Expression<Func<TestEntity, bool>> filter = e => e.Int < 50 &&
			//	e.NullableDouble >= 25 || e.String == "hello";

			Expression<Func<TestEntity, bool>> filter = e => e.NullableDouble == null &&
				e.NullableDouble >= 25 || e.String == "hello" || e.NullableDouble != e.NullableDouble2;


			//Expression<Func<TestEntity, bool>> filter = e => e.NullableDouble >= 25;

			var whereFilter = new WhereFilterResolver<TestEntity>();
			string result = whereFilter.FromExpression(filter);

			


			return;
			Console.ReadKey();
		}


		



		//public abstract class ExpressionTreeVisitor
		//{
		//	public ExpressionType NodeType => _node.NodeType;
		//	private readonly Expression _node;

		//	protected ExpressionTreeVisitor(Expression node)
		//	{
		//		_node = node;
		//	}

		//	protected abstract void Visit();

		//	public static ExpressionTreeVisitor FromExpression(Expression node)
		//	{
		//		switch (node)
		//		{
		//			case ConstantExpression constant:
		//				return new ConstantVisitor(constant);
		//			case LambdaExpression lambda:
		//				break;
		//			case ParameterExpression param:
		//				break;
		//			default:
		//				throw new ArgumentException($"Expression type '{node.NodeType}' is missing a visitor implementation.");
		//		}
		//	}
		//}

		//public class ConstantVisitor : ExpressionTreeVisitor
		//{
		//	private readonly ConstantExpression _node;

		//	public ConstantVisitor(ConstantExpression node)
		//		: base(node)
		//	{
		//		_node = node;
		//	}

		//	protected override void Visit()
		//	{
		//		Console.WriteLine($"Visiting a '{NodeType}' node:");
		//		Console.WriteLine($"    value = {_node.Value} type = {_node.Type}");
		//	}
		//}

		//public class LambdaVisitor : ExpressionTreeVisitor
		//{
		//	private readonly ConstantExpression _node;

		//	public LambdaVisitor(ConstantExpression node)
		//		: base(node)
		//	{
		//		_node = node;
		//	}

		//	protected override void Visit()
		//	{
		//		Console.WriteLine($"Visiting a '{NodeType}' node:");
		//		Console.WriteLine($"    value = {_node.Value} type = {_node.Type}");
		//	}
		//}

















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
