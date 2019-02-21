using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R5.FFDB.Components.CoreData;
using R5.FFDB.Components.CoreData.Static.Players.Add.Sources.V1;
using R5.FFDB.Components.Http;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
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

	public interface IFetchPlayersContext
	{
		List<string> FetchNflIds { get; set; }
	}

	public class FetchPlayersStage<TContext> : Stage<TContext>
		where TContext : IFetchPlayersContext
	{
		private IDatabaseProvider _dbProvider { get; }
		private WebRequestThrottle _throttle { get; }
		private IPlayerAddSource _playerAddSource { get; }

		public FetchPlayersStage(
			ILogger<FetchPlayersStage<TContext>> logger,
			IDatabaseProvider dbProvider,
			WebRequestThrottle throttle,
			IPlayerAddSource playerAddSource,
			int nestedDepth = 0)
			: base(logger, "Fetch Players", nestedDepth)
		{
			_dbProvider = dbProvider;
			_throttle = throttle;
			_playerAddSource = playerAddSource;
		}
		
		public override async Task<ProcessStageResult> ProcessAsync(TContext context)
		{
			Debug.Assert(context.FetchNflIds != null, $"'{nameof(context.FetchNflIds)}' list must be set before this stage runs.");

			if (!context.FetchNflIds.Any())
			{
				LogInformation("No players to add. Continuing to next stage.");
				return ProcessResult.Continue;
			}

			LogDebug($"Will fetch {context.FetchNflIds.Count} players.");
			
			IDatabaseContext dbContext = _dbProvider.GetContext();

			foreach(string nflId in context.FetchNflIds)
			{
				Debug.Assert(!TeamDataStore.IsTeam(nflId),
					$"NFL id '{nflId}' represents a team so shouldn't be fetched as a player.");

				SourceResult<PlayerAdd> result = await _playerAddSource.GetAsync(nflId);

				await dbContext.Player.AddAsync(result.Value);

				if (result.FetchedFromWeb)
				{
					await _throttle.DelayAsync();
				}

				LogInformation($"Successfully fetched '{nflId}'.");
			}
			
			return ProcessResult.Continue;
		}
	}
}
