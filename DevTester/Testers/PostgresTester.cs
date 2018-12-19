using Microsoft.Extensions.DependencyInjection;
using R5.FFDB.Components.CoreData.TeamData.Models;
using R5.FFDB.Core.Models;
using R5.FFDB.Database;
using R5.FFDB.DbProviders.PostgreSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTester.Testers
{
	internal static class PostgresTester
	{
		internal static async Task SetupTablesAsync(IServiceProvider serviceProvider)
		{
			var dbProvider = serviceProvider.GetRequiredService<IDatabaseProvider>();
			var dbContext = (PostgresDbContext)dbProvider.GetContext();

			//await dbContext.ExecuteCommandAsync(SetupCommands.CreateTeamsTable);
			//await dbContext.ExecuteCommandAsync(SetupCommands.CreatePlayersTable);
			//await dbContext.ExecuteCommandAsync(SetupCommands.CreatePlayerTeamMapTable);
			//await dbContext.ExecuteCommandAsync(CreateTableCommands.WeekStats());
		}

		internal static async Task InsertTeamsAsync(IServiceProvider serviceProvider)
		{
			var dbProvider = serviceProvider.GetRequiredService<IDatabaseProvider>();
			var dbContext = (PostgresDbContext)dbProvider.GetContext();

			// "INSERT INTO teams (id, nfl_id, name, abbreviation) VALUES (1, 100001, Atlanta Falcons, ATL)"
			List<Team> teams = TeamDataStore.GetAll();
			await dbContext.ExecuteCommandAsync(
				"INSERT INTO teams (id, nfl_id, name, abbreviation) VALUES (1, '100001', 'Atlanta Falcons', 'ATL')");

			//foreach(Team team in TeamDataStore.GetAll())
			//{
			//	string sqlCommand = InitialSeedCommands.Team(team);
			//}
		}
	}
}
