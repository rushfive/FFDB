using Microsoft.Extensions.Logging;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Entities;
using R5.FFDB.DbProviders.PostgreSql.Entities.WeekStats;
using R5.Internals.PostgresMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public class PlayerStatsDbContext : DbContextBase, IPlayerStatsDbContext
	{
		public PlayerStatsDbContext(DbConnection dbConnection, ILogger<PlayerStatsDbContext> logger)
			: base(dbConnection, logger)
		{
		}

		public Task AddAsync(List<PlayerWeekStats> stats)
		{

			foreach(PlayerWeekStats s in stats)
			{
				var passing = s.GetPassingStats();
				if (passing.Any())
				{

				}
			}

			throw new NotImplementedException();
		}

		

		public async Task<List<string>> GetPlayerNflIdsAsync(WeekInfo week)
		{
			Logger.LogDebug($"Getting all player's NFL ids that played in week '{week}'.");

			List<Guid> playerIds = await DbConnection.UnionSelect<Guid>()
				.From<WeekStatsIdpSql>(sb => sb.Property(s => s.PlayerId).Where(s => s.Season == week.Season && s.Week == week.Week))
				.From<WeekStatsKickSql>(sb => sb.Property(s => s.PlayerId).Where(s => s.Season == week.Season && s.Week == week.Week))
				.From<WeekStatsMiscSql>(sb => sb.Property(s => s.PlayerId).Where(s => s.Season == week.Season && s.Week == week.Week))
				.From<WeekStatsPassSql>(sb => sb.Property(s => s.PlayerId).Where(s => s.Season == week.Season && s.Week == week.Week))
				.From<WeekStatsReceiveSql>(sb => sb.Property(s => s.PlayerId).Where(s => s.Season == week.Season && s.Week == week.Week))
				.From<WeekStatsReturnSql>(sb => sb.Property(s => s.PlayerId).Where(s => s.Season == week.Season && s.Week == week.Week))
				.From<WeekStatsRushSql>(sb => sb.Property(s => s.PlayerId).Where(s => s.Season == week.Season && s.Week == week.Week))
				.ExecuteAsync();

			List<PlayerSql> players = await DbConnection
				.Select<PlayerSql>(
					p => p.Id,
					p => p.NflId)
				.ExecuteAsync();

			Dictionary<Guid, string> idMap = players.ToDictionary(p => p.Id, p => p.NflId);

			return playerIds
				.Where(id => idMap.ContainsKey(id))
				.Select(id => idMap[id])
				.ToList();
		}
	}
}