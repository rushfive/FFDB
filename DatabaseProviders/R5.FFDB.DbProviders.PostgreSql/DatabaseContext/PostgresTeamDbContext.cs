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
	public class PostgresTeamDbContext : PostgresDbContextBase, ITeamDatabaseContext
	{
		public PostgresTeamDbContext(
			Func<NpgsqlConnection> getConnection,
			ILoggerFactory loggerFactory)
			: base(getConnection, loggerFactory)
		{
		}

		public async Task AddTeamsAsync()
		{
			string tableName = EntityInfoMap.TableName(typeof(TeamSql));
			ILogger<PostgresTeamDbContext> logger = GetLogger<PostgresTeamDbContext>();

			logger.LogDebug($"Adding NFL team entries to '{tableName}' table.");

			IEnumerable<TeamSql> teamSqls = TeamDataStore
				.GetAll()
				.Select(TeamSql.FromCoreEntity);

			string insertTeamsSql = SqlCommandBuilder.Rows.InsertMany(teamSqls);

			logger.LogTrace("Adding teams using SQL command: " + Environment.NewLine + insertTeamsSql);

			await ExecuteNonQueryAsync(insertTeamsSql);

			logger.LogInformation($"Successfully added team entries to '{tableName}' table.");
		}

		// todo: DELETE all first??
		public async Task UpdateRostersAsync(List<Roster> rosters)
		{
			string playerTeamMapTableName = EntityInfoMap.TableName(typeof(PlayerTeamMapSql));
			var logger = GetLogger<PostgresTeamDbContext>();

			logger.LogDebug($"Adding player-team mapping entries to '{playerTeamMapTableName}' table (will first remove all existing entries).");

			string playerTableName = EntityInfoMap.TableName(typeof(PlayerSql));

			List<PlayerSql> players = await SelectAsEntitiesAsync<PlayerSql>($"SELECT id, nfl_id FROM {playerTableName};");
			Dictionary<string, Guid> nflIdMap = players.ToDictionary(p => p.NflId, p => p.Id);

			var sqlEntries = new List<PlayerTeamMapSql>();
			foreach(Roster roster in rosters)
			{
				foreach(RosterPlayer player in roster.Players)
				{
					Guid id = nflIdMap[player.NflId];
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
	}
}
