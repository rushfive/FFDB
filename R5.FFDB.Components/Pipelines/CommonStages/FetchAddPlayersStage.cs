using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R5.FFDB.Components.CoreData;
using R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Add;
using R5.FFDB.Components.Http;
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

	public interface IFetchAddPlayersContext
	{
		List<string> FetchAddNflIds { get; set; }
	}

	public class FetchAddPlayersStage<TContext> : Stage<TContext>
		where TContext : IFetchAddPlayersContext
	{
		private IDatabaseProvider _dbProvider { get; }
		private WebRequestThrottle _throttle { get; }
		private IPlayerAddSource _playerAddSource { get; }

		public FetchAddPlayersStage(
			ILogger<FetchAddPlayersStage<TContext>> logger,
			IDatabaseProvider dbProvider,
			WebRequestThrottle throttle,
			IPlayerAddSource playerAddSource)
			: base(logger, "Fetch and Save Players")
		{
			_dbProvider = dbProvider;
			_throttle = throttle;
			_playerAddSource = playerAddSource;
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
			
			IDatabaseContext dbContext = _dbProvider.GetContext();

			foreach(string nflId in context.FetchAddNflIds)
			{
				SourceResult<PlayerAdd> result = await _playerAddSource.GetAsync(nflId);

				await dbContext.Player.AddAsync(result.Value);

				if (result.FetchedFromWeb)
				{
					await _throttle.DelayAsync();
				}

				LogDebug($"Successfully fetched and saved '{nflId}'.");
			}
			
			return ProcessResult.Continue;
		}
	}
}
