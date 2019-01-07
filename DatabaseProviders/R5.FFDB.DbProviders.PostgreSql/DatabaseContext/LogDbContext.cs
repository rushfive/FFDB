using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Core.Models;
using R5.FFDB.Database.DbContext;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public class LogDbContext : DbContextBase, ILogDatabaseContext
	{
		public LogDbContext(
			Func<NpgsqlConnection> getConnection,
			ILoggerFactory loggerFactory)
			: base(getConnection, loggerFactory)
		{
		}

		public async Task AddUpdateForWeekAsync(WeekInfo week)
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
