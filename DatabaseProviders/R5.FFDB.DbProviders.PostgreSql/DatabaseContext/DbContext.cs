using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Core.Database;
using R5.Internals.PostgresMapper;
using System;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public class DbContext : DbContextBase, IDatabaseContext
	{
		public IPlayerDbContext Player { get; }
		public IPlayerStatsDbContext PlayerStats { get; }
		public ITeamDbContext Team { get; }
		public ITeamStatsDbContext TeamStats { get; }
		public IUpdateLogDbContext UpdateLog { get; }
		public IWeekMatchupsDbContext WeekMatchups { get; }

		public DbContext(
			Func<DbConnection> getDbConnection,
			ILoggerFactory loggerFactory)
			: base(getDbConnection, loggerFactory)
		{

		}

		public Task<bool> HasBeenInitializedAsync()
		{
			throw new NotImplementedException();
		}

		public Task InitializeAsync()
		{
			throw new NotImplementedException();
		}
	}

	public abstract class DbContextBase
	{
		protected Func<DbConnection> GetDbConnection { get; }
		private ILoggerFactory _loggerFactory { get; }

		protected DbContextBase(
			Func<DbConnection> getDbConnection,
			ILoggerFactory loggerFactory)
		{
			GetDbConnection = getDbConnection ?? throw new ArgumentNullException(nameof(getDbConnection));
			_loggerFactory = loggerFactory;
		}

		protected ILogger<T> GetLogger<T>()
		{
			return _loggerFactory.CreateLogger<T>();
		}
	}
}

//namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
//{
//	public class DbContext : DbContextBase//, IDatabaseContext
//	{
//		//public ITeamDatabaseContext Team { get; }
//		//public IPlayerDatabaseContext Player { get; }
//		//public IWeekStatsDatabaseContext Stats { get; }
//		//public ILogDatabaseContext Log { get; }

//		public DbContext(
//			Func<NpgsqlConnection> getConnection,
//			ILoggerFactory loggerFactory)
//			: base(getConnection, loggerFactory)
//		{
//			//Team = new TeamDbContext(getConnection, loggerFactory);
//			//Player = new PlayerDbContext(getConnection, loggerFactory);
//			//Stats = new WeekStatsDbContext(getConnection, loggerFactory);
//			//Log = new LogDbContext(getConnection, loggerFactory);
//		}


//		public Task<bool> HasBeenInitializedAsync()
//		{
//			string sqlCommand = "SELECT exists(select schema_name FROM information_schema.schemata WHERE schema_name = 'ffdb');";
//			throw new NotImplementedException();
//			//return ExecuteAsBoolAsync(sqlCommand);
//		}

//		public async Task InitializeAsync(bool force)
//		{
//			throw new NotImplementedException();
//			var logger = GetLogger<DbContext>();

//			logger.LogInformation("Creating postgresql schema 'ffdb'.");

//			//await ExecuteNonQueryAsync("CREATE SCHEMA ffdb;");

//			logger.LogDebug("Starting creation of database tables..");

//			foreach(Type entity in EntityMetadata.EntityTypes)
//			{
//				await CreateTableAsync(entity, logger);
//			}

//			logger.LogInformation("Successfully initialized database by creating schema and creating tables.");
//		}

//		private async Task CreateTableAsync(Type entityType, ILogger<DbContext> logger)
//		{
//			string tableName = EntityMetadata.TableName(entityType);
//			logger.LogDebug($"Creating table '{tableName}'.");

//			string sql = SqlCommandBuilder.Table.Create(entityType);
//			logger.LogTrace($"Adding using SQL command:" + Environment.NewLine + sql);

//			//await ExecuteNonQueryAsync(sql);
//			logger.LogInformation($"Successfully added table '{tableName}'.");
//		}
//	}
//}
