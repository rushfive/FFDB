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
using R5.Internals.PostgresMapper.Builders;
using System.Linq.Expressions;

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
			Expression<Func<int, int, int>> addition = (a, b) => a + b;

			// root node is type LambdaExpression

			// parameters? (left of =>)
			var addParams = addition.Parameters;

			// BinaryExpression in this case, but what if we dont know?
			
			Expression body = addition.Body; 
			// (a + b)	BinaryExpression
			// body.Left	ParameterExpression
			// body.Right	PArameterExpression









			ConstantExpression constant = Expression.Constant("wer", typeof(int));

			ExpressionType expressionType = constant.NodeType;
			object valueOf = constant.Value;
			Type valueType = constant.Type;

			switch (expressionType)
			{
				case ExpressionType.Add:
					break;
				case ExpressionType.AddAssign:
					break;
				case ExpressionType.AddAssignChecked:
					break;
				case ExpressionType.AddChecked:
					break;
				case ExpressionType.And:
					break;
				case ExpressionType.AndAlso:
					break;
				case ExpressionType.AndAssign:
					break;
				case ExpressionType.ArrayIndex:
					break;
				case ExpressionType.ArrayLength:
					break;
				case ExpressionType.Assign:
					break;
				case ExpressionType.Block:
					break;
				case ExpressionType.Call:
					break;
				case ExpressionType.Coalesce:
					break;
				case ExpressionType.Conditional:
					break;
				case ExpressionType.Constant:
					break;
				case ExpressionType.Convert:
					break;
				case ExpressionType.ConvertChecked:
					break;
				case ExpressionType.DebugInfo:
					break;
				case ExpressionType.Decrement:
					break;
				case ExpressionType.Default:
					break;
				case ExpressionType.Divide:
					break;
				case ExpressionType.DivideAssign:
					break;
				case ExpressionType.Dynamic:
					break;
				case ExpressionType.Equal:
					break;
				case ExpressionType.ExclusiveOr:
					break;
				case ExpressionType.ExclusiveOrAssign:
					break;
				case ExpressionType.Extension:
					break;
				case ExpressionType.Goto:
					break;
				case ExpressionType.GreaterThan:
					break;
				case ExpressionType.GreaterThanOrEqual:
					break;
				case ExpressionType.Increment:
					break;
				case ExpressionType.Index:
					break;
				case ExpressionType.Invoke:
					break;
				case ExpressionType.IsFalse:
					break;
				case ExpressionType.IsTrue:
					break;
				case ExpressionType.Label:
					break;
				case ExpressionType.Lambda:
					break;
				case ExpressionType.LeftShift:
					break;
				case ExpressionType.LeftShiftAssign:
					break;
				case ExpressionType.LessThan:
					break;
				case ExpressionType.LessThanOrEqual:
					break;
				case ExpressionType.ListInit:
					break;
				case ExpressionType.Loop:
					break;
				case ExpressionType.MemberAccess:
					break;
				case ExpressionType.MemberInit:
					break;
				case ExpressionType.Modulo:
					break;
				case ExpressionType.ModuloAssign:
					break;
				case ExpressionType.Multiply:
					break;
				case ExpressionType.MultiplyAssign:
					break;
				case ExpressionType.MultiplyAssignChecked:
					break;
				case ExpressionType.MultiplyChecked:
					break;
				case ExpressionType.Negate:
					break;
				case ExpressionType.NegateChecked:
					break;
				case ExpressionType.New:
					break;
				case ExpressionType.NewArrayBounds:
					break;
				case ExpressionType.NewArrayInit:
					break;
				case ExpressionType.Not:
					break;
				case ExpressionType.NotEqual:
					break;
				case ExpressionType.OnesComplement:
					break;
				case ExpressionType.Or:
					break;
				case ExpressionType.OrAssign:
					break;
				case ExpressionType.OrElse:
					break;
				case ExpressionType.Parameter:
					break;
				case ExpressionType.PostDecrementAssign:
					break;
				case ExpressionType.PostIncrementAssign:
					break;
				case ExpressionType.Power:
					break;
				case ExpressionType.PowerAssign:
					break;
				case ExpressionType.PreDecrementAssign:
					break;
				case ExpressionType.PreIncrementAssign:
					break;
				case ExpressionType.Quote:
					break;
				case ExpressionType.RightShift:
					break;
				case ExpressionType.RightShiftAssign:
					break;
				case ExpressionType.RuntimeVariables:
					break;
				case ExpressionType.Subtract:
					break;
				case ExpressionType.SubtractAssign:
					break;
				case ExpressionType.SubtractAssignChecked:
					break;
				case ExpressionType.SubtractChecked:
					break;
				case ExpressionType.Switch:
					break;
				case ExpressionType.Throw:
					break;
				case ExpressionType.Try:
					break;
				case ExpressionType.TypeAs:
					break;
				case ExpressionType.TypeEqual:
					break;
				case ExpressionType.TypeIs:
					break;
				case ExpressionType.UnaryPlus:
					break;
				case ExpressionType.Unbox:
					break;
			}

			string breakpoint = "";
			
			QueryBuilder.Test2<TestEntity, string>(
				e => e.String,
				s => s == "what"
				);
				
			
			return;
			Console.ReadKey();
		}

		public abstract class ExpressionTreeVisitor
		{
			public ExpressionType NodeType => _node.NodeType;
			private readonly Expression _node;

			protected ExpressionTreeVisitor(Expression node)
			{
				_node = node;
			}

			protected abstract void Visit();

			public static ExpressionTreeVisitor FromExpression(Expression node)
			{
				switch (node)
				{
					case ConstantExpression constant:
						return new ConstantVisitor(constant);
					case LambdaExpression lambda:
						break;
					case ParameterExpression param:
						break;
					default:
						throw new ArgumentException($"Expression type '{node.NodeType}' is missing a visitor implementation.");
				}
			}
		}

		public class ConstantVisitor : ExpressionTreeVisitor
		{
			private readonly ConstantExpression _node;

			public ConstantVisitor(ConstantExpression node)
				: base(node)
			{
				_node = node;
			}

			protected override void Visit()
			{
				Console.WriteLine($"Visiting a '{NodeType}' node:");
				Console.WriteLine($"    value = {_node.Value} type = {_node.Type}");
			}
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
