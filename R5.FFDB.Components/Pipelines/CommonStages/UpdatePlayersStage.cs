using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.Players;
using R5.FFDB.Components.CoreData.Rosters.Values;
using R5.FFDB.Components.Http;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.Core.Entities;
using R5.Lib.Pipeline;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Pipelines.CommonStages
{
	public interface IUpdatePlayersContext
	{
		List<string> UpdateNflIds { get; set; }
	}

	public class UpdatePlayersStage<TContext> : Stage<TContext>
		where TContext : IUpdatePlayersContext
	{
		private IDatabaseProvider _dbProvider { get; }
		private RostersValue _rosters { get; }
		//private IPlayerSource _playerSource { get; }
		private WebRequestThrottle _throttle { get; }

		public UpdatePlayersStage(
			ILogger<UpdatePlayersStage<TContext>> logger,
			IDatabaseProvider dbProvider,
			RostersValue rosters,
			//IPlayerSource playerSource,
			WebRequestThrottle throttle)
			: base(logger, "Update Players")
		{
			_dbProvider = dbProvider;
			_rosters = rosters;
			//_playerSource = playerSource;
			_throttle = throttle;
		}

		public override async Task<ProcessStageResult> ProcessAsync(TContext context)
		{
			Debug.Assert(context.UpdateNflIds != null, $"'{nameof(context.UpdateNflIds)}' list must be set before this stage runs.");

			if (!context.UpdateNflIds.Any())
			{
				LogInformation("No players to update. Continuing to next stage.");
				return ProcessResult.Continue;
			}

			LogDebug($"Will update {context.UpdateNflIds.Count} players.");

			Dictionary<string, RosterPlayer> rosterPlayerMap = await _rosters.GetPlayerMapAsync();

			foreach (string nflId in context.UpdateNflIds)
			{
				Player player = await FetchAsync(nflId, rosterPlayerMap);

				await UpdateAsync(player);

				await _throttle.DelayAsync();

				LogDebug($"Successfully updated '{nflId}' ({player.FirstName} {player.LastName})");
			}

			return ProcessResult.Continue;
		}

		private async Task<Player> FetchAsync(string nflId, Dictionary<string, RosterPlayer> rosterPlayerMap)
		{
			Player player = null;// await _playerSource.GetAsync(nflId);

			if (rosterPlayerMap.TryGetValue(nflId, out RosterPlayer rosterPlayer))
			{
				player.Number = rosterPlayer.Number;
				player.Position = rosterPlayer.Position;
				player.Status = rosterPlayer.Status;
			}

			return player;
		}

		private Task UpdateAsync(Player player)
		{
			IDatabaseContext dbContext = _dbProvider.GetContext();
			return dbContext.Player.UpdateAsync(player);
		}
	}
}
