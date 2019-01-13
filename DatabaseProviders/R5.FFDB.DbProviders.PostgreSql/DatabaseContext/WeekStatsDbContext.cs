using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Components.CoreData;
using R5.FFDB.Core.Entities;
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
		private static List<Type> _weekStatTypes = new List<Type>
		{
			typeof(WeekStatsPassSql),
			typeof(WeekStatsRushSql),
			typeof(WeekStatsReceiveSql),
			typeof(WeekStatsMiscSql),
			typeof(WeekStatsKickSql),
			typeof(WeekStatsDstSql),
			typeof(WeekStatsIdpSql)
		};

		public WeekStatsDbContext(
			Func<NpgsqlConnection> getConnection,
			ILoggerFactory loggerFactory)
			: base(getConnection, loggerFactory)
		{
		}
		
		public Task AddWeekAsync(WeekStats stats)
		{
			return AddWeeksAsync(new List<WeekStats> { stats });
		}

		public async Task AddWeeksAsync(List<WeekStats> stats)
		{
			var logger = GetLogger<WeekStatsDbContext>();

			List<PlayerSql> players = await SelectAsEntitiesAsync<PlayerSql>($"SELECT id, nfl_id FROM {EntityInfoMap.TableName(typeof(PlayerSql))};");

			var nflPlayerIdMap = players.ToDictionary(p => p.NflId, p => p.Id);
			var teamNflIdMap = TeamDataStore.GetAll().ToDictionary(t => t.NflId, t => t.Id);

			List<WeekStatsSqlAdd> statsAdd = GetStatsAdd(stats, nflPlayerIdMap, teamNflIdMap, logger);

			logger.LogInformation($"Adding week stats stats for {statsAdd.Count} week(s).");
			logger.LogTrace($"Adding week stats for: {string.Join(", ", stats.Select(s => s.Week))}");

			foreach(WeekStatsSqlAdd add in statsAdd)
			{
				logger.LogDebug($"Beginning stats add for week {add.Week}.");

				if (add.PassStats.Any())
				{
					await AddStatsAsync(add.PassStats, add.Week, "Week Stats (Pass)", logger);
				}
				if (add.RushStats.Any())
				{
					await AddStatsAsync(add.RushStats, add.Week, "Week Stats (Rush)", logger);
				}
				if (add.ReceiveStats.Any())
				{
					await AddStatsAsync(add.ReceiveStats, add.Week, "Week Stats (Receive)", logger);
				}
				if (add.MiscStats.Any())
				{
					await AddStatsAsync(add.MiscStats, add.Week, "Week Stats (Misc)", logger);
				}
				if (add.KickStats.Any())
				{
					await AddStatsAsync(add.KickStats, add.Week, "Week Stats (Kick)", logger);
				}
				if (add.DstStats.Any())
				{
					await AddStatsAsync(add.DstStats, add.Week, "Week Stats (DST)", logger);
				}
				if (add.IdpStats.Any())
				{
					await AddStatsAsync(add.IdpStats, add.Week, "Week Stats (IDP)", logger);
				}

				logger.LogDebug($"Successfully added stats for week {add.Week}.");
			}

			logger.LogInformation($"Successfully finished adding week stats for {statsAdd.Count} weeks.");
		}

		private static List<WeekStatsSqlAdd> GetStatsAdd(
			List<WeekStats> stats,
			Dictionary<string, Guid> nflPlayerIdMap,
			Dictionary<string, int> teamNflIdMap,
			ILogger<WeekStatsDbContext> logger)
		{
			var result = new List<WeekStatsSqlAdd>();

			foreach (WeekStats weekStats in stats)
			{
				var update = new WeekStatsSqlAdd
				{
					Week = weekStats.Week
				};

				foreach (PlayerWeekStats playerStats in weekStats.Players)
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

		private async Task AddStatsAsync<T>(List<T> items, WeekInfo week, string itemLabel, ILogger<WeekStatsDbContext> logger)
				where T : WeekStatsSql
		{
			var sqlCommand = SqlCommandBuilder.Rows.InsertMany(items);

			logger.LogTrace($"Adding '{itemLabel}' for {week} using SQL command:"
				+ Environment.NewLine + sqlCommand);

			await ExecuteNonQueryAsync(sqlCommand);

			logger.LogDebug($"Successfully added '{itemLabel}' for {week} ({items.Count} total rows).");
		}

		public async Task RemoveAllAsync()
		{
			var logger = GetLogger<WeekStatsDbContext>();
			logger.LogInformation("Removing all week stats rows from database.");

			foreach(Type type in _weekStatTypes)
			{
				await ExecuteNonQueryAsync(SqlCommandBuilder.Rows.DeleteAll(type));
			}

			logger.LogInformation("Successfully removed all week stats rows from database.");
		}

		public async Task RemoveForWeekAsync(WeekInfo week)
		{
			var logger = GetLogger<WeekStatsDbContext>();
			logger.LogInformation($"Removing week stats rows for {week} from database.");

			foreach(Type type in _weekStatTypes)
			{
				string tableName = EntityInfoMap.TableName(type);

				string sqlCommand = $"DELETE FROM {tableName} WHERE season = {week.Season} AND week = {week.Week};";

				await ExecuteNonQueryAsync(sqlCommand);
			}

			logger.LogInformation($"Successfully removed week stats rows for {week} from database.");
		}
	}
}
