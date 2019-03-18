using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Core.Database;
using R5.FFDB.DbProviders.PostgreSql.Entities;
using R5.FFDB.DbProviders.PostgreSql.Entities.WeekStats;
using R5.Internals.PostgresMapper;
using R5.Internals.PostgresMapper.System.Tables;
using System;
using System.Collections.Generic;
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
		
		public DbContext(DbConnection dbConnection, ILoggerFactory loggerFactory)
			: base(dbConnection, loggerFactory.CreateLogger<DbContext>())
		{
			Player = new PlayerDbContext(dbConnection, loggerFactory.CreateLogger<PlayerDbContext>());
			PlayerStats = new PlayerStatsDbContext(dbConnection, loggerFactory.CreateLogger<PlayerStatsDbContext>());
			Team = new TeamDbContext(dbConnection, loggerFactory.CreateLogger<TeamDbContext>());
			TeamStats = new TeamStatsDbContext(dbConnection, loggerFactory.CreateLogger<TeamStatsDbContext>());
			UpdateLog = new UpdateLogDbContext(dbConnection, loggerFactory.CreateLogger<UpdateLogDbContext>());
			WeekMatchups = new WeekMatchupsDbContext(dbConnection, loggerFactory.CreateLogger<WeekMatchupsDbContext>());
		}

		public Task<bool> HasBeenInitializedAsync()
		{
			return SchemaExistsAsync();
		}

		public async Task InitializeAsync()
		{
			bool schemaExists = await SchemaExistsAsync();
			if (!schemaExists)
			{
				Logger.LogInformation("Creating postgresql schema 'ffdb'.");
				await DbConnection.CreateSchema("ffdb");
			}

			Logger.LogInformation("Creating database tables.");

			await CreateTableAsync<TeamSql>();
			await CreateTableAsync<PlayerSql>();
			await CreateTableAsync<PlayerTeamMapSql>();
			await CreateTableAsync<WeekStatsPassSql>();
			await CreateTableAsync<WeekStatsRushSql>();
			await CreateTableAsync<WeekStatsReceiveSql>();
			await CreateTableAsync<WeekStatsMiscSql>();
			await CreateTableAsync<WeekStatsKickSql>();
			await CreateTableAsync<WeekStatsDstSql>();
			await CreateTableAsync<WeekStatsIdpSql>();
			await CreateTableAsync<WeekStatsReturnSql>();
			await CreateTableAsync<TeamGameStatsSql>();
			await CreateTableAsync<UpdateLogSql>();
			await CreateTableAsync<WeekGameMatchupSql>();

			Logger.LogInformation("Successfully initialized database by creating schema and creating tables.");
		}

		private async Task CreateTableAsync<TEntity>()
		{
			var tableName = MetadataResolver.TableName<TEntity>();

			bool exists = await DbConnection.TableExists<TEntity>().ExecuteAsync();
			if (exists)
			{
				Logger.LogDebug($"Table '{tableName}' already exists.");
			}
			else
			{
				await DbConnection.CreateTable<TEntity>().ExecuteAsync();
				Logger.LogDebug($"Created table '{tableName}'.");
			}
		}

		private Task<bool> SchemaExistsAsync()
		{
			return DbConnection.Exists<InformationSchema.Schemata>()
				.Where(s => s.SchemaName == "ffdb")
				.ExecuteAsync();
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
