using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Database;
using R5.FFDB.DbProviders.PostgreSql.Models.ColumnInfos;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public class PostgresDbContext : PostgresDbContextBase, IDatabaseContext
	{
		public ITeamDatabaseContext Team { get; }
		public IPlayerDatabaseContext Player { get; }
		public IWeekStatsDatabaseContext Stats { get; }

		public PostgresDbContext(
			Func<NpgsqlConnection> getConnection,
			ILoggerFactory loggerFactory)
			: base(getConnection, loggerFactory)
		{
			Team = new PostgresTeamDbContext(getConnection, loggerFactory);
			Player = new PostgresPlayerDbContext(getConnection, loggerFactory);
			Stats = new PostgresWeekStatsDbContext(getConnection, loggerFactory);
		}

		public async Task TestInsertWithParamsAsync()
		{

		}
		

		public async Task InitializeAsync()
		{
			var logger = GetLogger<PostgresDbContext>();

			logger.LogInformation("Creating postgresql schema 'ffdb'.");

			await ExecuteNonQueryAsync("CREATE SCHEMA ffdb;");

			logger.LogDebug("Starting creation of database tables..");

			await createTableAsync(typeof(TeamSql));
			await createTableAsync(typeof(PlayerSql));
			await createTableAsync(typeof(PlayerTeamMapSql));
			await createTableAsync(typeof(WeekStatsSql));
			await createTableAsync(typeof(WeekStatsKickerSql));
			await createTableAsync(typeof(WeekStatsDstSql));
			await createTableAsync(typeof(WeekStatsIdpSql));

			// local functions
			async Task createTableAsync(Type entityType)
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
}
