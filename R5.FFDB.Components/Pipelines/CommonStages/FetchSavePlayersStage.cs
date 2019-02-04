using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.Rosters.Values;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.Core.Entities;
using R5.Lib.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Pipelines.CommonStages
{
	public interface IPlayersListContext
	{
		List<string> NflIds { get; set; }
	}

	public class FetchSavePlayersStage : AsyncPipelineStage<IPlayersListContext>
	{
		private ILogger<FetchSavePlayersStage> _logger { get; }
		private IDatabaseProvider _dbProvider { get; }
		private RostersValue _rosters { get; }

		public FetchSavePlayersStage(
			ILogger<FetchSavePlayersStage> logger,
			IDatabaseProvider dbProvider,
			RostersValue rosters)
			: base("Fetch and Save Players")
		{
			_logger = logger;
			_dbProvider = dbProvider;
			_rosters = rosters;
		}

		public override async Task<ProcessStageResult> ProcessAsync(IPlayersListContext context)
		{
			List<string> requiredIds = await GetRequiredIdsAsync(context.NflIds);

			if (!requiredIds.Any())
			{
				_logger.LogInformation($"No new player profiles to add. "
					+ $"The {requiredIds.Count} players already exist in the database.");

				return ProcessResult.Continue;
			}

			_logger.LogDebug($"Will fetch and save {requiredIds.Count} players.");

			Dictionary<string, RosterPlayer> rosterPlayerMap = await _rosters.GetPlayerMapAsync();

			foreach(string nflId in requiredIds)
			{
				Player player = await FetchAsync(nflId);

				await SaveAsync(player, rosterPlayerMap);

				_logger.LogDebug($"Successfully fetched and saved '{nflId}' ({player.FirstName} {player.LastName})");
			}


			//
			await _dbProvider.GetContext().Stats.RemoveAllAsync();
			return ProcessResult.Continue;
		}

		// determine which players don't already exist in db
		private async Task<List<string>> GetRequiredIdsAsync(List<string> nflIds)
		{
			IDatabaseContext dbContext = _dbProvider.GetContext();

			HashSet<string> existingIds = (await dbContext.Player.GetAllAsync())
				.Select(p => p.NflId)
				.ToHashSet();

			return nflIds.Where(id => !existingIds.Contains(id)).ToList();
		}

		private async Task<Player> FetchAsync(string nflId)
		{
			// try resolving from disk
			// if fetch, persist based on program options
		}

		private async Task SaveAsync(Player player, Dictionary<string, RosterPlayer> rosterPlayerMap)
		{
				
		}
	}
}
