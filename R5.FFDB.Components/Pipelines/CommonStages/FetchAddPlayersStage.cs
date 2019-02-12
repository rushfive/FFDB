using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R5.FFDB.Components.Http;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.Core.Entities;
using R5.Lib.Pipeline;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Pipelines.CommonStages
{
	// this will simply fetch and add all players as specified in the ids list,
	// no filtering! this assumes all players in FetchAddNflIds have already
	// been verified to NOT already exist

	public interface IFetchAddPlayersContext
	{
		List<string> FetchAddNflIds { get; set; }
	}

	public class FetchAddPlayersStage<TContext> : Stage<TContext>
		where TContext : IFetchAddPlayersContext
	{
		private IDatabaseProvider _dbProvider { get; }
		//private RostersValue _rosters { get; }
		private DataDirectoryPath _dataPath { get; }
		//private IPlayerSource _playerSource { get; }
		private WebRequestThrottle _throttle { get; }

		public FetchAddPlayersStage(
			ILogger<FetchAddPlayersStage<TContext>> logger,
			IDatabaseProvider dbProvider,
			//RostersValue rosters,
			DataDirectoryPath dataPath,
			//IPlayerSource playerSource,
			WebRequestThrottle throttle)
			: base(logger, "Fetch and Save Players")
		{
			_dbProvider = dbProvider;
			//_rosters = rosters;
			_dataPath = dataPath;
			//_playerSource = playerSource;
			_throttle = throttle;
		}
		
		public override async Task<ProcessStageResult> ProcessAsync(TContext context)
		{
			Debug.Assert(context.FetchAddNflIds != null, $"'{nameof(context.FetchAddNflIds)}' list must be set before this stage runs.");

			if (!context.FetchAddNflIds.Any())
			{
				LogInformation("No players to add. Continuing to next stage.");
				return ProcessResult.Continue;
			}

			LogDebug($"Will fetch and save {context.FetchAddNflIds.Count} players.");

			Dictionary<string, RosterPlayer> rosterPlayerMap = null;// wait _rosters.GetPlayerMapAsync();

			foreach(string nflId in context.FetchAddNflIds)
			{
				Player player = await FetchAsync(nflId, rosterPlayerMap);
				
				await SaveAsync(player);
				
				await _throttle.DelayAsync();

				LogDebug($"Successfully fetched and saved '{nflId}' ({player.FirstName} {player.LastName})");
			}
			
			return ProcessResult.Continue;
		}
		
		private async Task<Player> FetchAsync(string nflId, Dictionary<string, RosterPlayer> rosterPlayerMap)
		{
			if (!TryGetFromDisk(nflId, out Player player))
			{
				//player = await _playerSource.GetAsync(nflId);
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

			string filePath = null;// _dataPath.Temp.Player + $"{nflId}.json";
			if (!File.Exists(filePath))
			{
				return false;
			}

			string serialized = File.ReadAllText(filePath);

			//PlayerJson json = JsonConvert.DeserializeObject<PlayerJson>(serialized);
			//player = PlayerJson.ToCoreEntity(json);

			return true;
		}

		private Task SaveAsync(Player player)
		{
			IDatabaseContext dbContext = _dbProvider.GetContext();
			return dbContext.Player.AddAsync(player);
		}
	}
}
