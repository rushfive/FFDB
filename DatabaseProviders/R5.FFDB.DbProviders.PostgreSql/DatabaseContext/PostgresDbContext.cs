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

		public async Task CreateTablesAsync()
		{
			string team = SqlCommandBuilder.Table.Create(typeof(TeamSql));
			string player = SqlCommandBuilder.Table.Create(typeof(PlayerSql));
			string playerTeamMap = SqlCommandBuilder.Table.Create(typeof(PlayerTeamMapSql));
			string weekStats = SqlCommandBuilder.Table.Create(typeof(WeekStatsSql));

			await ExecuteCommandAsync(team);
			await ExecuteCommandAsync(player);
			await ExecuteCommandAsync(playerTeamMap);
			await ExecuteCommandAsync(weekStats);
		}
	}
}
