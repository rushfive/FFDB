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
using R5.Internals.PostgresMapper.SqlBuilders;
using R5.Internals.Abstractions.Expressions;
using R5.Internals.PostgresMapper.Attributes;
using R5.Internals.PostgresMapper.QueryCommand;

namespace DevTester
{
	[Table("ffdb.team")]
	public class TestTmSql
	{
		[PrimaryKey]
		[Column("id", PostgresDataType.INT)]
		public int Id { get; set; }

		[NotNull]
		[Column("nfl_id", PostgresDataType.TEXT)]
		public string NflId { get; set; }

		[NotNull]
		[Column("name", PostgresDataType.TEXT)]
		public string Name { get; set; }

		[NotNull]
		[Column("abbreviation", PostgresDataType.TEXT)]
		public string Abbreviation { get; set; }
	}

	[Table("ffdb.NOT_EXISTS")]
	public class NotExists
	{
		public string LOL { get; set; }
	}

	[Table("cpk")]
	[CompositePrimaryKeys("stringCol", "intCol", "boolCol")]
	public class CompositePrimaryKeys
	{
		[Column("stringCol")]
		public string String { get; set; }
		[Column("intCol")]
		public int Int { get; set; }
		[Column("boolCol")]
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
			var name = MetadataResolver.TableName<CompositePrimaryKeys>();
			var cpk = MetadataResolver.CompositePrimaryKeys<CompositePrimaryKeys>();
			var cols = MetadataResolver.TableColumns<CompositePrimaryKeys>();

			var update = new UpdateCommand<CompositePrimaryKeys>(() => null);
			update
				.Set(c => c.String, "NEWSTR")
				.Set(c => c.Int, 33);

			var sqllll1 = update.GetSqlCommand();

			var update2 = new UpdateCommand<CompositePrimaryKeys>(() => null);
			update2
				.Set(c => c.String, "NEWSTR");

			var sqllll2 = update2.GetSqlCommand();

			var update3 = new UpdateCommand<CompositePrimaryKeys>(() => null);
			update3
				.Where(c => c.String != "what" && c.Int < 55)
				.Set(c => c.String, "NEWSTR")
				.Set(c => c.Int, 33);

			var sqllll13 = update3.GetSqlCommand();

			var update4 = new UpdateCommand<CompositePrimaryKeys>(() => null);
			update4
				.Where(c => c.String != "what" && c.Int < 55)
				.Set(c => c.String, "NEWSTR");

			var sqllll4 = update4.GetSqlCommand();

			var deleteWhere = new DeleteWhereCommand<CompositePrimaryKeys>(
				() => null,
				c => c.Int > 33 && c.String != "test");
			var deleWhereSql = deleteWhere.GetSqlCommand();

			var resolver = MetadataResolver.GetPrimaryKeyMatchConditionFunc<CompositePrimaryKeys>();

			var t1 = new CompositePrimaryKeys
			{
				Bool = false,
				Int = 33,
				String = "string for t1111111"
			};
			var t2 = new CompositePrimaryKeys
			{
				Bool = true,
				Int = 77,
				String = "string for 222"
			};

			var tEntity = new TestTmSql
			{
				Abbreviation = "abbr1",
				Id = 35,
				Name = "name1",
				NflId = "sadfasdf1"
			};
			var tEntity2 = new TestTmSql
			{
				Abbreviation = "abbr2",
				Id = 36,
				Name = "name2",
				NflId = "sadfasdf2"
			};

			var deleteCmd1 = new DeleteCommand<CompositePrimaryKeys>(
				() => null, t1);
			string sql1 = deleteCmd1.GetSqlCommand();

			var deleteCmd2 = new DeleteCommand<TestTmSql>(
				() => null, tEntity);
			string sql2 = deleteCmd2.GetSqlCommand();


			var resolved1 = resolver(t1);
			var resolved2 = resolver(t2);

			DbConnection dbConnection = GetPostgresDbConnection();

			var insertCmd = dbConnection.InsertMany(new List<TestTmSql>
			{
				tEntity,
					tEntity2
			});

			var sql = insertCmd.GetSqlCommand();

			//await insertCmd.ExecuteAsync();








			return;
			Console.ReadKey();
		}

		private static DbConnection GetPostgresDbConnection()
		{
			return new DbConnection(NpgsqlConnectionFactory);

		}

		private static NpgsqlConnection NpgsqlConnectionFactory()
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

		public class Other
		{
			public bool OtherBool { get; set; }
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
