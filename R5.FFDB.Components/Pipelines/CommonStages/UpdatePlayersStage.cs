using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData;
using R5.FFDB.Components.CoreData.Static.Players;
using R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Update;
using R5.FFDB.Components.Http;
using R5.FFDB.Core.Database;
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
		private WebRequestThrottle _throttle { get; }
		private IPlayerUpdateSource _playerUpdateSource { get; }
		private IPlayerIdMappings _playerIdMappings { get; }

		public UpdatePlayersStage(
			ILogger<UpdatePlayersStage<TContext>> logger,
			IDatabaseProvider dbProvider,
			WebRequestThrottle throttle,
			IPlayerUpdateSource playerUpdateSource,
			IPlayerIdMappings playerIdMappings)
			: base(logger, "Update Players")
		{
			_dbProvider = dbProvider;
			_throttle = throttle;
			_playerUpdateSource = playerUpdateSource;
			_playerIdMappings = playerIdMappings;
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

			IDatabaseContext dbContext = _dbProvider.GetContext();
			Dictionary<string, Guid> nflIdMap = await _playerIdMappings.GetNflToIdMapAsync();

			foreach (string nflId in context.UpdateNflIds)
			{
				if (!nflIdMap.TryGetValue(nflId, out Guid id))
				{
					LogWarning($"Failed to find player '{nflId}' in database. Will skip update.");
					continue;
				}

				SourceResult<PlayerUpdate> result = await _playerUpdateSource.GetAsync(nflId);

				await dbContext.Player.UpdateAsync(id, result.Value);

				if (result.FetchedFromWeb)
				{
					await _throttle.DelayAsync();
				}

				LogDebug($"Successfully updated '{nflId}'.");
			}

			return ProcessResult.Continue;
		}
	}
}
