using Microsoft.Extensions.Logging;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Entities;
using R5.FFDB.DbProviders.PostgreSql.Entities.WeekStats;
using R5.Internals.Extensions.Collections;
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

		public async Task AddAsync(List<PlayerWeekStats> stats)
		{
			if (stats.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(stats), "Stats must be provided.");
			}

			Logger.LogDebug($"Adding player week stats for '{stats[0].Week}'.");

			Dictionary<string, Guid> idMap = await GetNflIdMapAsync();

			await Task.WhenAll(
				AddPassingStatsAsync(stats, idMap),
				AddRushingStatsAsync(stats, idMap),
				AddReceivingStatsAsync(stats, idMap),
				AddReturnStatsAsync(stats, idMap),
				AddMiscStatsAsync(stats, idMap),
				AddKickingStatsAsync(stats, idMap),
				AddIdpStatsAsync(stats, idMap),
				AddDstStatsAsync(stats, idMap));
		}

		private async Task<Dictionary<string, Guid>> GetNflIdMapAsync()
		{
			List<PlayerSql> players = await DbConnection
				.Select<PlayerSql>(
					p => p.Id,
					p => p.NflId)
				.ExecuteAsync();

			return players.ToDictionary(p => p.NflId, p => p.Id);
		}

		private Task AddPassingStatsAsync(List<PlayerWeekStats> stats, Dictionary<string, Guid> idMap)
		{
			List<WeekStatsPassSql> sqlEntries = stats
				.Select(s => s.TryGetPassSql(idMap, out WeekStatsPassSql sql) ? sql : null)
				.Where(s => s != null)
				.ToList();

			return sqlEntries.Any()
				? DbConnection.InsertMany(sqlEntries).ExecuteAsync()
				: Task.CompletedTask;
		}

		private Task AddRushingStatsAsync(List<PlayerWeekStats> stats, Dictionary<string, Guid> idMap)
		{
			List<WeekStatsRushSql> sqlEntries = stats
				.Select(s => s.TryGetRushSql(idMap, out WeekStatsRushSql sql) ? sql : null)
				.Where(s => s != null)
				.ToList();

			return sqlEntries.Any()
				? DbConnection.InsertMany(sqlEntries).ExecuteAsync()
				: Task.CompletedTask;
		}

		private Task AddReceivingStatsAsync(List<PlayerWeekStats> stats, Dictionary<string, Guid> idMap)
		{
			List<WeekStatsReceiveSql> sqlEntries = stats
				.Select(s => s.TryGetReceiveSql(idMap, out WeekStatsReceiveSql sql) ? sql : null)
				.Where(s => s != null)
				.ToList();

			return sqlEntries.Any()
				? DbConnection.InsertMany(sqlEntries).ExecuteAsync()
				: Task.CompletedTask;
		}

		private Task AddReturnStatsAsync(List<PlayerWeekStats> stats, Dictionary<string, Guid> idMap)
		{
			List<WeekStatsReturnSql> sqlEntries = stats
				.Select(s => s.TryGetReturnSql(idMap, out WeekStatsReturnSql sql) ? sql : null)
				.Where(s => s != null)
				.ToList();

			return sqlEntries.Any()
				? DbConnection.InsertMany(sqlEntries).ExecuteAsync()
				: Task.CompletedTask;
		}

		private Task AddMiscStatsAsync(List<PlayerWeekStats> stats, Dictionary<string, Guid> idMap)
		{
			List<WeekStatsMiscSql> sqlEntries = stats
				.Select(s => s.TryGetMiscSql(idMap, out WeekStatsMiscSql sql) ? sql : null)
				.Where(s => s != null)
				.ToList();

			return sqlEntries.Any()
				? DbConnection.InsertMany(sqlEntries).ExecuteAsync()
				: Task.CompletedTask;
		}

		private Task AddKickingStatsAsync(List<PlayerWeekStats> stats, Dictionary<string, Guid> idMap)
		{
			List<WeekStatsKickSql> sqlEntries = stats
				.Select(s => s.TryGetKickSql(idMap, out WeekStatsKickSql sql) ? sql : null)
				.Where(s => s != null)
				.ToList();

			return sqlEntries.Any()
				? DbConnection.InsertMany(sqlEntries).ExecuteAsync()
				: Task.CompletedTask;
		}

		private Task AddIdpStatsAsync(List<PlayerWeekStats> stats, Dictionary<string, Guid> idMap)
		{
			List<WeekStatsIdpSql> sqlEntries = stats
				.Select(s => s.TryGetIdpSql(idMap, out WeekStatsIdpSql sql) ? sql : null)
				.Where(s => s != null)
				.ToList();

			return sqlEntries.Any()
				? DbConnection.InsertMany(sqlEntries).ExecuteAsync()
				: Task.CompletedTask;
		}

		private Task AddDstStatsAsync(List<PlayerWeekStats> stats, Dictionary<string, Guid> idMap)
		{
			List<WeekStatsDstSql> sqlEntries = stats
				.Select(s => s.TryGetDstSql(idMap, out WeekStatsDstSql sql) ? sql : null)
				.Where(s => s != null)
				.ToList();

			return sqlEntries.Any() 
				? DbConnection.InsertMany(sqlEntries).ExecuteAsync() 
				: Task.CompletedTask;
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