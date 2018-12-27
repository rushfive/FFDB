using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Components.CoreData.TeamData.Models;
using R5.FFDB.Core.Models;
using R5.FFDB.Database;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public class PostgresWeekStatsDbContext : PostgresDbContextBase, IWeekStatsDatabaseContext
	{
		public PostgresWeekStatsDbContext(
			Func<NpgsqlConnection> getConnection,
			ILoggerFactory loggerFactory)
			: base(getConnection, loggerFactory)
		{
		}

		public Task<List<WeekInfo>> GetExistingWeeksAsync()
		{
			throw new NotImplementedException();
		}

		public async Task UpdateWeeksAsync(List<WeekStats> stats)
		{
			List<PlayerSql> players = await SelectAsEntitiesAsync<PlayerSql>($"SELECT id, nfl_id FROM {EntityInfoMap.TableName(typeof(PlayerSql))};");
			Dictionary<string, Guid> nflIdMap = players.ToDictionary(p => p.NflId, p => p.Id);

			// temp: move DST stats to a separate table?
			var teamNflIds = TeamDataStore.GetAll().Select(t => t.NflId).ToHashSet();

			var weekStatSqls = new List<WeekStatsSql>();
			foreach (WeekStats s in stats)
			{
				foreach(PlayerStats p in s.Players)
				{
					if (teamNflIds.Contains(p.NflId))
					{
						Console.WriteLine($"Skipping '{p.NflId}' because its a team.");
						continue;
					}

					WeekStatsSql statsSql = WeekStatsSql.FromCoreEntity(p, nflIdMap[p.NflId], s.Week);
					weekStatSqls.Add(statsSql);
				}
			}

			var test = "s";

			throw new NotImplementedException();
		}
	}
}
