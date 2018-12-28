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
			var logger = GetLogger<PostgresWeekStatsDbContext>();

			List<PlayerSql> players = await SelectAsEntitiesAsync<PlayerSql>($"SELECT id, nfl_id FROM {EntityInfoMap.TableName(typeof(PlayerSql))};");

			var nflPlayerIdMap = players.ToDictionary(p => p.NflId, p => p.Id);
			var teamNflIdMap = TeamDataStore.GetAll().ToDictionary(t => t.NflId, t => t.Id);

			List<WeekStatsSqlUpdate> updates = GetStatsUpdates(stats, nflPlayerIdMap, teamNflIdMap, logger);

			logger.LogInformation($"Updating game stats for {updates.Count} weeks.");

			foreach(var update in updates)
			{
				logger.LogDebug($"Beginning stats update for week {update.Week}.");

				if (update.WeekStats.Any())
				{
					await updateStatsAsync(update.WeekStats, update.Week, "Week Stats");
				}
				if (update.WeekStatsKicker.Any())
				{
					await updateStatsAsync(update.WeekStatsKicker, update.Week, "Week Stats (Kicker)");
				}
				if (update.WeekStatsDst.Any())
				{
					await updateStatsAsync(update.WeekStatsDst, update.Week, "Week Stats (DST)");
				}
				if (update.WeekStatsIdp.Any())
				{
					await updateStatsAsync(update.WeekStatsIdp, update.Week, "Week Stats (IDP)");
				}

				logger.LogDebug($"Successfully updated stats for week {update.Week}.");
			}

			logger.LogInformation($"Successfully finished updating game stats for {updates.Count} weeks.");

			// local functions
			async Task updateStatsAsync<T>(List<T> items, WeekInfo week, string itemLabel)
				where T : WeekStatsSqlBase
			{
				var sqlCommand = SqlCommandBuilder.Rows.InsertMany(items);

				logger.LogTrace($"Updating '{itemLabel}' for {week} using SQL command:"
					+ Environment.NewLine + sqlCommand);

				await ExecuteNonQueryAsync(sqlCommand);

				logger.LogDebug($"Successfully updated '{itemLabel}' for {week} ({items.Count} total rows).");
			}
		}

		public class WeekStatsSqlUpdate
		{
			public WeekInfo Week { get; set; }
			public List<WeekStatsSql> WeekStats { get; } = new List<WeekStatsSql>();
			public List<WeekStatsKickerSql> WeekStatsKicker { get; } = new List<WeekStatsKickerSql>();
			public List<WeekStatsDstSql> WeekStatsDst { get; } = new List<WeekStatsDstSql>();
			public List<WeekStatsIdpSql> WeekStatsIdp { get; } = new List<WeekStatsIdpSql>();
		}

		private static List<WeekStatsSqlUpdate> GetStatsUpdates(
			List<WeekStats> stats,
			Dictionary<string, Guid> nflPlayerIdMap,
			Dictionary<string, int> teamNflIdMap,
			ILogger<PostgresWeekStatsDbContext> logger)
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
							update.WeekStatsDst.Add(statsSql);
						}

						continue;
					}

					if (nflPlayerIdMap.TryGetValue(playerStats.NflId, out Guid playerId))
					{
						List<WeekStatsPlayerSqlBase> playerStatSqls = WeekStatsPlayerSqlBase.FromCoreEntity(playerStats, playerId, weekStats.Week);

						foreach (var sql in playerStatSqls)
						{
							switch (sql)
							{
								case WeekStatsSql ws:
									update.WeekStats.Add(ws);
									break;
								case WeekStatsKickerSql wsKicker:
									update.WeekStatsKicker.Add(wsKicker);
									break;
								case WeekStatsIdpSql wsIdp:
									update.WeekStatsIdp.Add(wsIdp);
									break;
								default:
									throw new ArgumentOutOfRangeException(nameof(sql), $"'{sql.GetType().Name}' is an invalid '{nameof(WeekStatsPlayerSqlBase)}' type.");
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
	}
}
