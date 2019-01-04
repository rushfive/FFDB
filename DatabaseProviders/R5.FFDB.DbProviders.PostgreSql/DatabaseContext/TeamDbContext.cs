using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Components.CoreData.TeamData.Models;
using R5.FFDB.Core.Models;
using R5.FFDB.Database;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public class TeamDbContext : DbContextBase, ITeamDatabaseContext
	{
		public TeamDbContext(
			Func<NpgsqlConnection> getConnection,
			ILoggerFactory loggerFactory)
			: base(getConnection, loggerFactory)
		{
		}

		public async Task AddTeamsAsync()
		{
			string tableName = EntityInfoMap.TableName(typeof(TeamSql));
			ILogger<TeamDbContext> logger = GetLogger<TeamDbContext>();

			logger.LogDebug($"Adding NFL team entries to '{tableName}' table.");

			IEnumerable<TeamSql> teamSqls = TeamDataStore
				.GetAll()
				.Select(TeamSql.FromCoreEntity);

			string insertTeamsSql = SqlCommandBuilder.Rows.InsertMany(teamSqls);

			logger.LogTrace("Adding teams using SQL command: " + Environment.NewLine + insertTeamsSql);

			await ExecuteNonQueryAsync(insertTeamsSql);

			logger.LogInformation($"Successfully added team entries to '{tableName}' table.");
		}
		
		public async Task UpdateRostersAsync(List<Roster> rosters)
		{
			string playerTeamMapTableName = EntityInfoMap.TableName(typeof(PlayerTeamMapSql));
			var logger = GetLogger<TeamDbContext>();

			logger.LogDebug($"Adding player-team mapping entries to '{playerTeamMapTableName}' table (will first remove all existing entries).");

			string playerTableName = EntityInfoMap.TableName(typeof(PlayerSql));

			List<PlayerSql> players = await SelectAsEntitiesAsync<PlayerSql>($"SELECT id, nfl_id FROM {playerTableName};");
			Dictionary<string, Guid> nflIdMap = players.ToDictionary(p => p.NflId, p => p.Id);

			var sqlEntries = new List<PlayerTeamMapSql>();
			foreach(Roster roster in rosters)
			{
				foreach(RosterPlayer player in roster.Players)
				{
					if (!nflIdMap.TryGetValue(player.NflId, out Guid id))
					{
						logger.LogWarning($"Player with NFL id '{player.NflId}' was found on the team '{roster.TeamAbbreviation}' page "
							+ "but the player profile was unable to be fetched and resolved. Try updating rosters again later.");
						continue;
					}
					
					sqlEntries.Add(PlayerTeamMapSql.ToSqlEntity(id, roster.TeamId));
				}
			}
			
			string truncateCommand = $"TRUNCATE {playerTeamMapTableName};";
			string insertCommands = SqlCommandBuilder.Rows.InsertMany(sqlEntries);

			await ExecuteTransactionWrappedAsync(new List<string>
			{
				truncateCommand, insertCommands
			});

			logger.LogInformation($"Successfully added player-team mapping entries to '{playerTeamMapTableName}' table.");
		}

		public async Task UpdateGameStatsAsync(List<TeamWeekStats> stats)
		{
			string tableName = EntityInfoMap.TableName(typeof(TeamGameStatsSql));
			var logger = GetLogger<TeamDbContext>();

			logger.LogDebug($"Adding team game stats to '{tableName}' table.");

			List<TeamGameStatsSql> sqlEntries = stats.Select(TeamGameStatsSql.FromCoreEntity).ToList();

			var sqlCommand = SqlCommandBuilder.Rows.InsertMany(sqlEntries);

			await ExecuteNonQueryAsync(sqlCommand);
			logger.LogInformation($"Successfully added team game stats to '{tableName}' table.");
		}
	}
}
