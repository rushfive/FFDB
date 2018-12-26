using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Database;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public class PostgresDbContext : PostgresDbContextBase, IDatabaseContext
	{
		public ITeamDatabaseContext Team { get; }
		public IPlayerDatabaseContext Player { get; }
		public IWeekStatsDatabaseContext Stats { get; }

		public PostgresDbContext(
			Func<NpgsqlConnection> getConnection,
			ILoggerFactory loggerFactory)
			: base(getConnection, loggerFactory)
		{
			Team = new PostgresTeamDbContext(getConnection, loggerFactory);
			Player = new PostgresPlayerDbContext(getConnection, loggerFactory);
			Stats = new PostgresWeekStatsDbContext(getConnection, loggerFactory);
		}

		public async Task TestInsertWithParamsAsync()
		{
			string command = "INSERT INTO players VALUES " +
				$"('{Guid.NewGuid()}', 'TEST_NFL_ID', 'TEST_ESB_ID', 'TEST_GSIS', "
				+ $"1, @FirstName, @LastName, 'QB', 33, 55, 66, '1996-01-10', @College)";

			var sqlParams = new List<(string, string)>
			{
				("@FirstName", "TestFirstName Hello'McFattyPants"),
				("@LastName", "TestLastName Last`ConnieWhat"),
				("@College", "Test O'Donals McVersa-tay`ee")
			};

			await ExecuteCommandAsync(command, sqlParams);
		}

		public async Task CreateTablesAsync()
		{
			var logger = GetLogger<PostgresDbContext>();
			logger.LogDebug("Starting creation of database tables..");

			await createTableAsync(typeof(TeamSql));
			await createTableAsync(typeof(PlayerSql));
			await createTableAsync(typeof(PlayerTeamMapSql));
			await createTableAsync(typeof(WeekStatsSql));

			// local functions
			async Task createTableAsync(Type entityType)
			{
				string tableName = EntityInfoMap.TableName(entityType);
				logger.LogDebug($"Creating table '{tableName}'.");

				string sql = SqlCommandBuilder.Table.Create(entityType);
				logger.LogTrace($"Adding using SQL command:" + Environment.NewLine + sql);

				await ExecuteCommandAsync(sql);
				logger.LogInformation($"Successfully added table '{tableName}'.");
			}
		}
	}
}
