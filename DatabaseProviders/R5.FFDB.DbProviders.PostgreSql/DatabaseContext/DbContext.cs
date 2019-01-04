using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Core.Models;
using R5.FFDB.Database;
using R5.FFDB.DbProviders.PostgreSql.Models.ColumnInfos;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities.WeekStats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public class DbContext : DbContextBase, IDatabaseContext
	{
		public ITeamDatabaseContext Team { get; }
		public IPlayerDatabaseContext Player { get; }
		public IWeekStatsDatabaseContext Stats { get; }

		public DbContext(
			Func<NpgsqlConnection> getConnection,
			ILoggerFactory loggerFactory)
			: base(getConnection, loggerFactory)
		{
			Team = new TeamDbContext(getConnection, loggerFactory);
			Player = new PlayerDbContext(getConnection, loggerFactory);
			Stats = new WeekStatsDbContext(getConnection, loggerFactory);
		}
		

		public async Task InitializeAsync()
		{
			var logger = GetLogger<DbContext>();

			logger.LogInformation("Creating postgresql schema 'ffdb'.");

			await ExecuteNonQueryAsync("CREATE SCHEMA ffdb;");

			logger.LogDebug("Starting creation of database tables..");

			await createTableAsync(typeof(UpdateLogSql));
			await createTableAsync(typeof(TeamSql));
			await createTableAsync(typeof(PlayerSql));
			await createTableAsync(typeof(PlayerTeamMapSql));
			await createTableAsync(typeof(WeekStatsPassSql));
			await createTableAsync(typeof(WeekStatsRushSql));
			await createTableAsync(typeof(WeekStatsReceiveSql));
			await createTableAsync(typeof(WeekStatsMiscSql));
			await createTableAsync(typeof(WeekStatsKickSql));
			await createTableAsync(typeof(WeekStatsDstSql));
			await createTableAsync(typeof(WeekStatsIdpSql));
			await createTableAsync(typeof(TeamGameStatsSql));

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

		public async Task AddUpdateLogAsync(WeekInfo week)
		{
			string tableName = EntityInfoMap.TableName(typeof(UpdateLogSql));
			var logger = GetLogger<DbContext>();

			var log = new UpdateLogSql
			{
				Season = week.Season,
				Week = week.Week,
				UpdateTime = DateTime.UtcNow
			};

			var sql = SqlCommandBuilder.Rows.Insert(log);

			logger.LogTrace($"Adding update log for {week} using SQL command: " + Environment.NewLine + sql);

			await ExecuteNonQueryAsync(sql);

			logger.LogInformation($"Successfully added update log for {week} to '{tableName}' table.");
		}

		public async Task<List<WeekInfo>> GetUpdatedWeeksAsync()
		{
			var logs = await SelectAsEntitiesAsync<UpdateLogSql>();
			return logs.Select(sql => new WeekInfo(sql.Season, sql.Week)).ToList();
		}
	}
}
