using Microsoft.Extensions.DependencyInjection;
using R5.FFDB.Components.CoreData;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.FFDB.Database;
using R5.FFDB.DbProviders.PostgreSql;
using R5.FFDB.DbProviders.PostgreSql.DatabaseContext;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities.WeekStats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTester.Testers
{
	internal static class PostgresTester
	{
		internal static async Task TestSetupAsync(IServiceProvider serviceProvider)
		{
			var dbProvider = serviceProvider.GetRequiredService<IDatabaseProvider>();
			var dbContext = (DbContext)dbProvider.GetContext();

			string team = SqlCommandBuilder.Table.Create(typeof(TeamSql));
			string player = SqlCommandBuilder.Table.Create(typeof(PlayerSql));
			string playerTeamMap = SqlCommandBuilder.Table.Create(typeof(PlayerTeamMapSql));
			//string weekStats = SqlCommandBuilder.Table.Create(typeof(WeekStatsSql));

			await dbContext.ExecuteNonQueryAsync(team);
			await dbContext.ExecuteNonQueryAsync(player);
			await dbContext.ExecuteNonQueryAsync(playerTeamMap);
			//await dbContext.ExecuteNonQueryAsync(weekStats);

			// insert teams
			IEnumerable<TeamSql> teamSqls = TeamDataStore
				.GetAll()
				.Select(TeamSql.FromCoreEntity);

			string insertTeamsSql = SqlCommandBuilder.Rows.InsertMany(teamSqls);
			await dbContext.ExecuteNonQueryAsync(insertTeamsSql);
		}


		internal static async Task SetupTablesAsync(IServiceProvider serviceProvider)
		{
			var dbProvider = serviceProvider.GetRequiredService<IDatabaseProvider>();
			var dbContext = (DbContext)dbProvider.GetContext();

			//await dbContext.ExecuteCommandAsync(SetupCommands.CreateTeamsTable);
			//await dbContext.ExecuteCommandAsync(SetupCommands.CreatePlayersTable);
			//await dbContext.ExecuteCommandAsync(SetupCommands.CreatePlayerTeamMapTable);
			//await dbContext.ExecuteCommandAsync(CreateTableCommands.WeekStats());
		}

		internal static async Task InsertTeamsAsync(IServiceProvider serviceProvider)
		{
			var dbProvider = serviceProvider.GetRequiredService<IDatabaseProvider>();
			var dbContext = (DbContext)dbProvider.GetContext();

			// "INSERT INTO teams (id, nfl_id, name, abbreviation) VALUES (1, 100001, Atlanta Falcons, ATL)"
			List<Team> teams = TeamDataStore.GetAll();
			await dbContext.ExecuteNonQueryAsync(
				"INSERT INTO teams (id, nfl_id, name, abbreviation) VALUES (1, '100001', 'Atlanta Falcons', 'ATL')");

			//foreach(Team team in TeamDataStore.GetAll())
			//{
			//	string sqlCommand = InitialSeedCommands.Team(team);
			//}
		}

		internal static void OutCreateTableSqlCommands()
		{
			string team = SqlCommandBuilder.Table.Create(typeof(TeamSql));
			string player = SqlCommandBuilder.Table.Create(typeof(PlayerSql));
			string playerTeamMap = SqlCommandBuilder.Table.Create(typeof(PlayerTeamMapSql));
			//string weekStats = SqlCommandBuilder.Table.Create(typeof(WeekStatsSql));

			//string breaks = Environment.NewLine + Environment.NewLine;
			//Console.WriteLine($"{team}{breaks}{player}{breaks}{playerTeamMap}{breaks}{weekStats}");
		}

		internal static void OutputInsertSqlCommandsForTeams(bool insertMany)
		{
			//IEnumerable<TeamSql> teamSqls = TeamDataStore
			//	.GetAll()
			//	.Select(TeamSql.FromCoreEntity);

			//if (insertMany)
			//{
			//	string sql = SqlCommandBuilder.Rows.InsertMany(teamSqls);
			//	Console.WriteLine(sql);
			//}
			//else
			//{
			//	foreach (TeamSql team in teamSqls)
			//	{
			//		string insertSql = team.InsertCommand();
			//		Console.WriteLine($"{insertSql}{Environment.NewLine}{Environment.NewLine}");
			//	}
			//}
		}

		internal static void OutputInsertSqlCommandsForPlayers()
		{



			//IEnumerable<TeamSql> teamSqls = TeamDataStore
			//	.GetAll()
			//	.Select(TeamSql.FromCoreEntity);

			//foreach (TeamSql team in teamSqls)
			//{
			//	string insertSql = team.InsertCommand();
			//	Console.WriteLine($"{insertSql}{Environment.NewLine}{Environment.NewLine}");
			//}
		}
	}
}
