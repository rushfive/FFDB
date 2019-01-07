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
using R5.FFDB.Database.DbContext;
using R5.FFDB.DbProviders.PostgreSql.Models;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities.WeekStats;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public class WeekStatsDbContext : DbContextBase, IWeekStatsDatabaseContext
	{
		public WeekStatsDbContext(
			Func<NpgsqlConnection> getConnection,
			ILoggerFactory loggerFactory)
			: base(getConnection, loggerFactory)
		{
		}

		public Task<List<WeekInfo>> GetExistingWeeksAsync()
		{
			throw new NotImplementedException();
		}

		public Task UpdateWeekAsync(WeekStats stats)
		{
			return UpdateWeeksAsync(new List<WeekStats> { stats });
		}

		public async Task UpdateWeeksAsync(List<WeekStats> stats)
		{
			var logger = GetLogger<WeekStatsDbContext>();

			List<PlayerSql> players = await SelectAsEntitiesAsync<PlayerSql>($"SELECT id, nfl_id FROM {EntityInfoMap.TableName(typeof(PlayerSql))};");

			var nflPlayerIdMap = players.ToDictionary(p => p.NflId, p => p.Id);
			var teamNflIdMap = TeamDataStore.GetAll().ToDictionary(t => t.NflId, t => t.Id);

			List<WeekStatsSqlUpdate> updates = GetStatsUpdates(stats, nflPlayerIdMap, teamNflIdMap, logger);

			logger.LogInformation($"Updating game stats for {updates.Count} week(s).");

			foreach(var update in updates)
			{
				logger.LogDebug($"Beginning stats update for week {update.Week}.");

				if (update.PassStats.Any())
				{
					await UpdateStatsAsync(update.PassStats, update.Week, "Week Stats (Pass)", logger);
				}
				if (update.RushStats.Any())
				{
					await UpdateStatsAsync(update.RushStats, update.Week, "Week Stats (Rush)", logger);
				}
				if (update.ReceiveStats.Any())
				{
					await UpdateStatsAsync(update.ReceiveStats, update.Week, "Week Stats (Receive)", logger);
				}
				if (update.MiscStats.Any())
				{
					await UpdateStatsAsync(update.MiscStats, update.Week, "Week Stats (Misc)", logger);
				}
				if (update.KickStats.Any())
				{
					await UpdateStatsAsync(update.KickStats, update.Week, "Week Stats (Kick)", logger);
				}
				if (update.DstStats.Any())
				{
					await UpdateStatsAsync(update.DstStats, update.Week, "Week Stats (DST)", logger);
				}
				if (update.IdpStats.Any())
				{
					await UpdateStatsAsync(update.IdpStats, update.Week, "Week Stats (IDP)", logger);
				}

				logger.LogDebug($"Successfully updated stats for week {update.Week}.");
			}

			logger.LogInformation($"Successfully finished updating game stats for {updates.Count} weeks.");
		}

		private static List<WeekStatsSqlUpdate> GetStatsUpdates(
			List<WeekStats> stats,
			Dictionary<string, Guid> nflPlayerIdMap,
			Dictionary<string, int> teamNflIdMap,
			ILogger<WeekStatsDbContext> logger)
		{
			var result = new List<WeekStatsSqlUpdate>();

			foreach (WeekStats weekStats in stats)
			{
				var update = new WeekStatsSqlUpdate
				{
					Week = weekStats.Week
				};

				foreach (PlayerStats playerStats in weekStats.Players)
				{
					if (teamNflIdMap.TryGetValue(playerStats.NflId, out int teamId))
					{
						var statValues = WeekStatsDstSql.FilterStatValues(playerStats);
						if (statValues.Any())
						{
							WeekStatsDstSql statsSql = WeekStatsDstSql.FromCoreEntity(teamId, weekStats.Week, statValues);
							update.DstStats.Add(statsSql);
						}

						continue;
					}

					if (nflPlayerIdMap.TryGetValue(playerStats.NflId, out Guid playerId))
					{
						List<WeekStatsPlayerSql> playerStatSqls = WeekStatsPlayerSql.FromCoreEntity(playerStats, playerId, weekStats.Week);

						foreach (var sql in playerStatSqls)
						{
							switch (sql)
							{
								case WeekStatsPassSql pass:
									update.PassStats.Add(pass);
									break;
								case WeekStatsRushSql rush:
									update.RushStats.Add(rush);
									break;
								case WeekStatsReceiveSql receive:
									update.ReceiveStats.Add(receive);
									break;
								case WeekStatsMiscSql misc:
									update.MiscStats.Add(misc);
									break;
								case WeekStatsKickSql kick:
									update.KickStats.Add(kick);
									break;
								case WeekStatsIdpSql idp:
									update.IdpStats.Add(idp);
									break;
								default:
									throw new ArgumentOutOfRangeException(nameof(sql), $"'{sql.GetType().Name}' is an invalid '{nameof(WeekStatsPlayerSql)}' type.");
							}
						}

						continue;
					}

					logger.LogWarning($"Failed to map NFL id '{playerStats.NflId}' to either a Team id or Player id. "
						+ $"They have stats recorded for week {weekStats.Week.Week} ({weekStats.Week.Season}) but cannot be added to the database.");
				}

				result.Add(update);
			}

			return result;
		}

		private async Task UpdateStatsAsync<T>(List<T> items, WeekInfo week, string itemLabel, ILogger<WeekStatsDbContext> logger)
				where T : WeekStatsSql
		{
			var sqlCommand = SqlCommandBuilder.Rows.InsertMany(items);

			logger.LogTrace($"Updating '{itemLabel}' for {week} using SQL command:"
				+ Environment.NewLine + sqlCommand);

			await ExecuteNonQueryAsync(sqlCommand);

			logger.LogDebug($"Successfully updated '{itemLabel}' for {week} ({items.Count} total rows).");
		}
	}
}
