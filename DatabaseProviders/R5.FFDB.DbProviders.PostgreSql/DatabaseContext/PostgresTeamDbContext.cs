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

			await ExecuteCommandAsync(insertTeamsSql);

			logger.LogInformation($"Successfully added team entries to '{tableName}' table.");
		}

		public Task UpdateRostersAsync(List<Roster> rosters)
		{
			throw new NotImplementedException();
		}
	}
}
