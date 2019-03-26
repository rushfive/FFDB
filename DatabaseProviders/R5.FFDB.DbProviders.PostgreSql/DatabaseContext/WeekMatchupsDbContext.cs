using Microsoft.Extensions.Logging;
using R5.FFDB.Components;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Entities;
using R5.Internals.PostgresMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public class WeekMatchupsDbContext : DbContextBase, IWeekMatchupsDbContext
	{
		public WeekMatchupsDbContext(DbConnection dbConnection, IAppLogger logger)
			: base(dbConnection, logger)
		{
		}

		public async Task<List<WeekMatchup>> GetAsync(WeekInfo week)
		{
			Logger.LogDebug($"Retrieving week matchups for week '{week}' from '{MetadataResolver.TableName<WeekGameMatchupSql>()}' table.");

			List<WeekGameMatchupSql> matchups = await DbConnection.Select<WeekGameMatchupSql>()
				.Where(m => m.Season == week.Season && m.Week == week.Week)
				.ExecuteAsync();
			
			return matchups.Select(WeekGameMatchupSql.ToCoreEntity).ToList();
		}

		public Task AddAsync(List<WeekMatchup> matchups)
		{
			if (matchups == null)
			{
				throw new ArgumentNullException(nameof(matchups), "Week matchups must be provided.");
			}
			
			Logger.LogDebug($"Adding {matchups.Count} week matchups to '{MetadataResolver.TableName<WeekGameMatchupSql>()}' table.");

			var sqlEntries = matchups.Select(WeekGameMatchupSql.FromCoreEntity).ToList();

			return DbConnection.InsertMany(sqlEntries).ExecuteAsync();
		}
	}
}
