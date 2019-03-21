using Microsoft.Extensions.Logging;
using Npgsql;
using R5.FFDB.Components;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.FFDB.DbProviders.PostgreSql.Entities;
using R5.Internals.PostgresMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R5.FFDB.DbProviders.PostgreSql.DatabaseContext
{
	public class TeamDbContext : DbContextBase, ITeamDbContext
	{
		public TeamDbContext(DbConnection dbConnection, IAppLogger logger)
			: base(dbConnection, logger)
		{
		}

		public async Task AddAsync(List<Team> teams)
		{
			if (teams == null)
			{
				throw new ArgumentNullException(nameof(teams), "Teams must be provided.");
			}

			// todo: move this checking of existing to the engine. dbContext shouldnt care about this kind of logic
			HashSet<int> existing = await GetExistingTeamIdsAsync();

			List<TeamSql> missing = teams
				.Where(t => !existing.Contains(t.Id))
				.Select(TeamSql.FromCoreEntity)
				.ToList();

			if (!missing.Any())
			{
				return;
			}

			Logger.LogDebug($"Adding {missing.Count} teams to '{MetadataResolver.TableName<TeamSql>()}' table.");

			await DbConnection.InsertMany(missing).ExecuteAsync();
		}

		private async Task<HashSet<int>> GetExistingTeamIdsAsync()
		{
			List<TeamSql> existing = await DbConnection.Select<TeamSql>(t => t.Id).ExecuteAsync();
			return existing.Select(t => t.Id).ToHashSet();
		}
		
		public async Task UpdateRosterMappingsAsync(List<Roster> rosters)
		{
			if (rosters == null)
			{
				throw new ArgumentNullException(nameof(rosters), "Rosters must be provided.");
			}

			Logger.LogDebug($"Updating roster mappings for players in '{MetadataResolver.TableName<PlayerTeamMapSql>()}'.");

			await DbConnection.Truncate<PlayerTeamMapSql>().ExecuteAsync();

			List<PlayerSql> players = await DbConnection.Select<PlayerSql>(p => p.Id, p => p.NflId).ExecuteAsync();
			Dictionary<string, Guid> nflIdMap = players.ToDictionary(p => p.NflId, p => p.Id);

			foreach (Roster roster in rosters)
			{
				await UpdateForRosterAsync(roster, nflIdMap);
			}
		}
		

		private Task UpdateForRosterAsync(Roster roster, Dictionary<string, Guid> nflIdMap)
		{
			List<PlayerTeamMapSql> entries = roster.Players
				.Where(p => nflIdMap.ContainsKey(p.NflId))
				.Select(p => PlayerTeamMapSql.ToSqlEntity(nflIdMap[p.NflId], roster.TeamId))
				.ToList();

			return DbConnection.InsertMany(entries.ToList()).ExecuteAsync();
		}
	}
}