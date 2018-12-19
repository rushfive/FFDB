using Npgsql;
using R5.FFDB.Core.Models;
using R5.FFDB.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.PostgreSql
{
	public class PostgresDbContext : PostgresDbContextBase, IDatabaseContext
	{
		public ITeamDatabaseContext Team { get; }
		public IPlayerDatabaseContext Player { get; }
		public IStatsDatabaseContext Stats { get; }

		protected override Func<NpgsqlConnection> _getConnection { get; }

		public PostgresDbContext(Func<NpgsqlConnection> getConnection)
		{
			_getConnection = getConnection;
			Team = new PostgresTeamDbContext(getConnection);
			Player = new PostgresPlayerDbContext(getConnection);
			Stats = new PostgresStatsDbContext(getConnection);
		}

		public async Task RunInitialSetupAsync()
		{
			await Task.WhenAll(
				ExecuteCommandAsync(CreateTableCommands.Teams),
				ExecuteCommandAsync(CreateTableCommands.Players)
			);

			await ExecuteCommandAsync(CreateTableCommands.WeekStats());
		}

		
		
	}

	public abstract class PostgresDbContextBase
	{
		protected abstract Func<NpgsqlConnection> _getConnection { get; }

		// todo base class
		// todo transactions
		public async Task ExecuteCommandAsync(string sqlCommand, List<(string key, string value)> parameters = null)
		{
			using (NpgsqlConnection connection = _getConnection())
			{
				await connection.OpenAsync();

				using (var command = new NpgsqlCommand())
				{
					command.Connection = connection;
					command.CommandText = sqlCommand;

					if (parameters?.Any() ?? false)
					{
						parameters.ForEach(p => command.Parameters.AddWithValue(p.key, p.value));
					}

					await command.ExecuteNonQueryAsync();
				}
			}
		}
	}

	public class PostgresTeamDbContext : PostgresDbContextBase, ITeamDatabaseContext
	{
		protected override Func<NpgsqlConnection> _getConnection { get; }

		public PostgresTeamDbContext(Func<NpgsqlConnection> getConnection)
		{
			_getConnection = getConnection;
		}

		public Task UpdateRostersAsync(List<Roster> rosters)
		{
			throw new NotImplementedException();
		}
	}

	public class PostgresPlayerDbContext : PostgresDbContextBase, IPlayerDatabaseContext
	{
		protected override Func<NpgsqlConnection> _getConnection { get; }

		public PostgresPlayerDbContext(Func<NpgsqlConnection> getConnection)
		{
			_getConnection = getConnection;
		}

		public Task<List<PlayerProfile>> GetExistingAsync()
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(List<PlayerProfile> players, bool overrideExisting)
		{
			throw new NotImplementedException();
		}
	}

	public class PostgresStatsDbContext : PostgresDbContextBase, IStatsDatabaseContext
	{
		protected override Func<NpgsqlConnection> _getConnection { get; }

		public PostgresStatsDbContext(Func<NpgsqlConnection> getConnection)
		{
			_getConnection = getConnection;
		}

		public Task<List<WeekInfo>> GetExistingWeeksAsync()
		{
			throw new NotImplementedException();
		}

		public Task UpdateWeeksAsync(List<WeekStats> stats, bool overrideExisting)
		{
			throw new NotImplementedException();
		}
	}

	

	public class PostgresDbProvider : IDatabaseProvider
	{
		private PostgresConfig _config { get; }

		public PostgresDbProvider(PostgresConfig config)
		{
			_config = config;
		}

		public IDatabaseContext GetContext()
		{
			return new PostgresDbContext(GetConnection);
		}

		private NpgsqlConnection GetConnection()
		{
			string connectionString = $"Host={_config.Host};Database={_config.DatabaseName};";

			if (_config.IsSecured)
			{
				connectionString += $"Username={_config.Username};Password={_config.Password}";
			}

			return new NpgsqlConnection(connectionString);
		}
	}

	// todo: validations
	public class PostgresConfig
	{
		public string Host { get; set; }
		public string DatabaseName { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }

		public bool IsSecured => !string.IsNullOrWhiteSpace(Username)
			&& !string.IsNullOrWhiteSpace(Password);
	}
}
