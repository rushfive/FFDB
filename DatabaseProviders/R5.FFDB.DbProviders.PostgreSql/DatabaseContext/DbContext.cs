using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Components;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
using R5.FFDB.DbProviders.PostgreSql.Entities;
using R5.FFDB.DbProviders.PostgreSql.Entities.WeekStats;
using R5.Internals.PostgresMapper;
using R5.Internals.PostgresMapper.System.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public class DbContext : DbContextBase, IDatabaseContext
	{
		public IPlayerDbContext Player { get; }
		public IPlayerStatsDbContext PlayerStats { get; }
		public ITeamDbContext Team { get; }
		public ITeamStatsDbContext TeamStats { get; }
		public IUpdateLogDbContext UpdateLog { get; }
		public IWeekMatchupsDbContext WeekMatchups { get; }
		
		public DbContext(DbConnection dbConnection, IAppLogger logger)
			: base(dbConnection, logger)
		{
			Player = new PlayerDbContext(dbConnection, logger);
			PlayerStats = new PlayerStatsDbContext(dbConnection, logger);
			Team = new TeamDbContext(dbConnection, logger);
			TeamStats = new TeamStatsDbContext(dbConnection, logger);
			UpdateLog = new UpdateLogDbContext(dbConnection, logger);
			WeekMatchups = new WeekMatchupsDbContext(dbConnection, logger);
		}

		public async Task<bool> HasBeenInitializedAsync()
		{
			var requiredInitChecks = new List<Func<Task<bool>>>
			{
				() => SchemaExistsAsync(),
				() => AllRequiredTablesExistAsync(),
				async () =>
				{
					var existingTeams = (await Team.GetExistingTeamIdsAsync()).ToHashSet();
					return Teams.GetAll().Select(t => t.Id).All(id => existingTeams.Contains(id));
				}
			};

			foreach (var checkFunc in requiredInitChecks)
			{
				if (!await checkFunc())
				{
					return false;
				}
			}

			return true;
		}

		private async Task<bool> AllRequiredTablesExistAsync()
		{
			var checks = new List<Task<bool>>
			{
				DbConnection.TableExists<TeamSql>().ExecuteAsync(),
				DbConnection.TableExists<PlayerSql>().ExecuteAsync(),
				DbConnection.TableExists<PlayerTeamMapSql>().ExecuteAsync(),
				DbConnection.TableExists<WeekStatsPassSql>().ExecuteAsync(),
				DbConnection.TableExists<WeekStatsRushSql>().ExecuteAsync(),
				DbConnection.TableExists<WeekStatsReceiveSql>().ExecuteAsync(),
				DbConnection.TableExists<WeekStatsMiscSql>().ExecuteAsync(),
				DbConnection.TableExists<WeekStatsKickSql>().ExecuteAsync(),
				DbConnection.TableExists<WeekStatsDstSql>().ExecuteAsync(),
				DbConnection.TableExists<WeekStatsIdpSql>().ExecuteAsync(),
				DbConnection.TableExists<WeekStatsReturnSql>().ExecuteAsync(),
				DbConnection.TableExists<TeamGameStatsSql>().ExecuteAsync(),
				DbConnection.TableExists<UpdateLogSql>().ExecuteAsync(),
				DbConnection.TableExists<WeekGameMatchupSql>().ExecuteAsync()
			};

			foreach(var checkTask in checks)
			{
				if (!await checkTask)
				{
					return false;
				}
			}

			return true;
		}

		public async Task InitializeAsync()
		{
			bool schemaExists = await SchemaExistsAsync();
			if (!schemaExists)
			{
				Logger.LogInformation("Creating postgresql schema 'ffdb'.");
				await DbConnection.CreateSchema("ffdb");
			}

			Logger.LogInformation("Creating database tables.");

			// this ordering of table creations must be preserved (for FKs)
			await CreateTableAsync<TeamSql>();
			await CreateTableAsync<PlayerSql>();
			await CreateTableAsync<PlayerTeamMapSql>();
			await CreateTableAsync<WeekStatsPassSql>();
			await CreateTableAsync<WeekStatsRushSql>();
			await CreateTableAsync<WeekStatsReceiveSql>();
			await CreateTableAsync<WeekStatsMiscSql>();
			await CreateTableAsync<WeekStatsKickSql>();
			await CreateTableAsync<WeekStatsDstSql>();
			await CreateTableAsync<WeekStatsIdpSql>();
			await CreateTableAsync<WeekStatsReturnSql>();
			await CreateTableAsync<TeamGameStatsSql>();
			await CreateTableAsync<UpdateLogSql>();
			await CreateTableAsync<WeekGameMatchupSql>();

			Logger.LogInformation("Successfully initialized database by creating schema and creating tables.");
		}

		private async Task CreateTableAsync<TEntity>()
		{
			var tableName = MetadataResolver.TableName<TEntity>();

			bool exists = await DbConnection.TableExists<TEntity>().ExecuteAsync();
			if (exists)
			{
				Logger.LogDebug($"Table '{tableName}' already exists.");
			}
			else
			{
				await DbConnection.CreateTable<TEntity>().ExecuteAsync();
				Logger.LogDebug($"Created table '{tableName}'.");
			}
		}

		private Task<bool> SchemaExistsAsync()
		{
			return DbConnection.Exists<InformationSchema.Schemata>()
				.Where(s => s.SchemaName == "ffdb")
				.ExecuteAsync();
		}
	}	
}