using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R5.FFDB.Components.CoreData.Players;
using R5.FFDB.Components.CoreData.Players.Models;
using R5.FFDB.Components.CoreData.Rosters.Values;
using R5.FFDB.Components.Http;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.Core.Entities;
using R5.Lib.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
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
		private DataDirectoryPath _dataPath { get; }
		private IPlayerSource _playerSource { get; }
		private WebRequestThrottle _throttle { get; }

		public FetchSavePlayersStage(
			ILogger<FetchSavePlayersStage> logger,
			IDatabaseProvider dbProvider,
			RostersValue rosters,
			DataDirectoryPath dataPath,
			IPlayerSource playerSource,
			WebRequestThrottle throttle)
			: base("Fetch and Save Players")
		{
			_logger = logger;
			_dbProvider = dbProvider;
			_rosters = rosters;
			_dataPath = dataPath;
			_playerSource = playerSource;
			_throttle = throttle;
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
				Player player = await FetchAsync(nflId, rosterPlayerMap);
				
				await SaveAsync(player);
				
				await _throttle.DelayAsync();

				_logger.LogDebug($"Successfully fetched and saved '{nflId}' ({player.FirstName} {player.LastName})");
			}
			
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

		private async Task<Player> FetchAsync(string nflId, Dictionary<string, RosterPlayer> rosterPlayerMap)
		{
			if (!TryGetFromDisk(nflId, out Player player))
			{
				player = await _playerSource.GetAsync(nflId);
			}

			if (rosterPlayerMap.TryGetValue(nflId, out RosterPlayer rosterPlayer))
			{
				player.Number = rosterPlayer.Number;
				player.Position = rosterPlayer.Position;
				player.Status = rosterPlayer.Status;
			}

			return player;
		}

		private bool TryGetFromDisk(string nflId, out Player player)
		{
			player = null;

			string filePath = _dataPath.Temp.Player + $"{nflId}.json";
			if (!File.Exists(filePath))
			{
				return false;
			}

			string serialized = File.ReadAllText(filePath);

			PlayerJson json = JsonConvert.DeserializeObject<PlayerJson>(serialized);
			player = PlayerJson.ToCoreEntity(json);

			return true;
		}

		private Task SaveAsync(Player player)
		{
			IDatabaseContext dbContext = _dbProvider.GetContext();
			return dbContext.Player.AddAsync(player);
		}
	}
}
