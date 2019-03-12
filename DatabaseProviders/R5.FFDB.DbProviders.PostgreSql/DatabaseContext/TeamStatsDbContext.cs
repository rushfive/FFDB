using Microsoft.Extensions.Logging;
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
	public class TeamStatsDbContext : DbContextBase, ITeamStatsDbContext
	{
		public TeamStatsDbContext(DbConnection dbConnection, ILogger<TeamStatsDbContext> logger)
			: base(dbConnection, logger)
		{
		}

		public async Task<List<TeamWeekStats>> GetAsync(WeekInfo week)
		{
			Logger.LogDebug($"Getting team stats for '{week}' from '{MetadataResolver.TableName<TeamGameStatsSql>()}' table.");

			List<TeamGameStatsSql> sqlEntries = await DbConnection.Select<TeamGameStatsSql>()
				.Where(s => s.Season == week.Season && s.Week == week.Week)
				.ExecuteAsync();

			return sqlEntries.Select(TeamGameStatsSql.ToCoreEntity).ToList();
		}

		public Task AddAsync(List<TeamWeekStats> stats)
		{
			if (stats == null)
			{
				throw new ArgumentNullException(nameof(stats), "Stats must be provided.");
			}

			Logger.LogDebug($"Adding {stats.Count} team stats entries for '{stats.First().Week}' to '{MetadataResolver.TableName<TeamGameStatsSql>()}' table.");

			List<TeamGameStatsSql> sqlEntries = stats.Select(TeamGameStatsSql.FromCoreEntity).ToList();

			return DbConnection.InsertMany(sqlEntries).ExecuteAsync();
		}
	}
}
