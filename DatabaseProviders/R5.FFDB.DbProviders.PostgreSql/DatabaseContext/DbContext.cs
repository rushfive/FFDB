using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Database.DbContext;
using System;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public class DbContext : DbContextBase, IDatabaseContext
	{
		public ITeamDatabaseContext Team { get; }
		public IPlayerDatabaseContext Player { get; }
		public IWeekStatsDatabaseContext Stats { get; }
		public ILogDatabaseContext Log { get; }

		public DbContext(
			Func<NpgsqlConnection> getConnection,
			ILoggerFactory loggerFactory)
			: base(getConnection, loggerFactory)
		{
			Team = new TeamDbContext(getConnection, loggerFactory);
			Player = new PlayerDbContext(getConnection, loggerFactory);
			Stats = new WeekStatsDbContext(getConnection, loggerFactory);
			Log = new LogDbContext(getConnection, loggerFactory);
		}
		

		public async Task InitializeAsync()
		{
			var logger = GetLogger<DbContext>();

			logger.LogInformation("Creating postgresql schema 'ffdb'.");

			await ExecuteNonQueryAsync("CREATE SCHEMA ffdb;");

			logger.LogDebug("Starting creation of database tables..");

			foreach(Type entity in EntityInfoMap.EntityTypes)
			{
				await CreateTableAsync(entity, logger);
			}

			logger.LogInformation("Successfully initialized database by creating schema and creating tables.");
		}

		private async Task CreateTableAsync(Type entityType, ILogger<DbContext> logger)
		{
			string tableName = EntityInfoMap.TableName(entityType);
			logger.LogDebug($"Creating table '{tableName}'.");

			string sql = SqlCommandBuilder.Table.Create(entityType);
			logger.LogTrace($"Adding using SQL command:" + Environment.NewLine + sql);

			await ExecuteNonQueryAsync(sql);
			logger.LogInformation($"Successfully added table '{tableName}'.");
		}
	}
}
