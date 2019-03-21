using Microsoft.Extensions.Logging;
using R5.FFDB.Components;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Entities;
using R5.Internals.PostgresMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public class UpdateLogDbContext : DbContextBase, IUpdateLogDbContext
	{
		public UpdateLogDbContext(DbConnection dbConnection, IAppLogger logger)
			: base(dbConnection, logger)
		{
		}

		public async Task<List<WeekInfo>> GetAsync()
		{
			List<UpdateLogSql> logs = await DbConnection.Select<UpdateLogSql>().ExecuteAsync();

			return logs.Select(l => new WeekInfo(l.Season, l.Week)).ToList();
		}

		public Task AddAsync(WeekInfo week)
		{
			Logger.LogDebug("Adding update log for {Week}.", week);

			var log = new UpdateLogSql
			{
				Season = week.Season,
				Week = week.Week,
				UpdateTime = DateTime.Now
			};

			return DbConnection.Insert(log).ExecuteAsync();
		}

		public Task<bool> HasUpdatedWeekAsync(WeekInfo week)
		{
			return DbConnection.Exists<UpdateLogSql>()
				.Where(l => l.Season == week.Season && l.Week == week.Week)
				.ExecuteAsync();
		}
	}
}