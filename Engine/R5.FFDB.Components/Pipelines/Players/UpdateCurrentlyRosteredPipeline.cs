using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.Dynamic.Rosters;
using R5.FFDB.Components.CoreData.Static.Players;
using R5.FFDB.Components.Extensions;
using R5.FFDB.Components.Pipelines.CommonStages;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.Internals.Extensions.DependencyInjection;
using R5.Lib.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Pipelines.Players
{
	public class UpdateCurrentlyRosteredPipeline : Pipeline<UpdateCurrentlyRosteredPipeline.Context>
	{
		private ILogger<UpdateCurrentlyRosteredPipeline> _logger { get; }

		public UpdateCurrentlyRosteredPipeline(
			ILogger<UpdateCurrentlyRosteredPipeline> logger,
			AsyncPipelineStage<Context> head)
			: base(logger, head, "Update Currently Rostered Players")
		{
			_logger = logger;
		}


		public class Context : IFetchPlayersContext
		{
			public List<string> FetchNflIds { get; set; }
			public List<string> UpdateNflIds { get; set; }
		}

		public static UpdateCurrentlyRosteredPipeline Create(IServiceProvider sp)
		{
			var groupByNewExisting = sp.Create<Stages.GroupByNewAndExisting>();
			var fetchSavePlayers = sp.Create<FetchPlayersStage<Context>>();
			var updatePlayers = sp.Create<Stages.UpdatePlayersStage>();

			AsyncPipelineStage<Context> chain = groupByNewExisting;
			chain
				.SetNext(fetchSavePlayers)
				.SetNext(updatePlayers);

			return sp.Create<UpdateCurrentlyRosteredPipeline>(chain);
		}

		public static class Stages
		{
			public class GroupByNewAndExisting : Stage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }
				private IRosterCache _rosterCache { get; }

				public GroupByNewAndExisting(
					ILogger<GroupByNewAndExisting> logger,
					IDatabaseProvider dbProvider,
					IRosterCache rosterCache)
					: base(logger, "Group by New and Existing")
				{
					_dbProvider = dbProvider;
					_rosterCache = rosterCache;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					IDatabaseContext dbContext = _dbProvider.GetContext();

					HashSet<string> existingPlayers = (await dbContext.Player.GetAllAsync())
						.Select(p => p.NflId)
						.ToHashSet(StringComparer.OrdinalIgnoreCase);

					List<string> rosteredIds = await _rosterCache.GetRosteredIdsAsync();

					var newIds = new List<string>();
					var existingIds = new List<string>();

					foreach(string id in rosteredIds)
					{
						if (existingPlayers.Contains(id))
						{
							existingIds.Add(id);
						}
						else
						{
							newIds.Add(id);
						}
					}
					
					context.FetchNflIds = newIds;
					context.UpdateNflIds = existingIds;

					return ProcessResult.Continue;
				}
			}

			public class UpdatePlayersStage : Stage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }
				private IRosterCache _rosterCache { get; }
				private IPlayerIdMappings _playerIdMappings { get; }

				public UpdatePlayersStage(
					ILogger<UpdatePlayersStage> logger,
					IDatabaseProvider dbProvider,
					IRosterCache rosterCache,
					IPlayerIdMappings playerIdMappings)
					: base(logger, "Update Players")
				{
					_dbProvider = dbProvider;
					_rosterCache = rosterCache;
					_playerIdMappings = playerIdMappings;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					IDatabaseContext dbContext = _dbProvider.GetContext();
					Dictionary<string, Guid> nflIdMap = await _playerIdMappings.GetNflToIdMapAsync();

					foreach (var nflId in context.UpdateNflIds)
					{
						var playerData = await _rosterCache.GetPlayerDataAsync(nflId);

						if (!playerData.HasValue)
						{
							// log
							continue;
						}

						if (!nflIdMap.TryGetValue(nflId, out Guid id))
						{
							LogWarning($"Failed to find player '{nflId}' in database. Will skip update.");
							continue;
						}

						var update = new PlayerUpdate
						{
							Number = playerData.Value.number,
							Position = playerData.Value.position,
							Status = playerData.Value.status
						};

						await dbContext.Player.UpdateAsync(id, update);
					}

					return ProcessResult.Continue;
				}
			}
		}
	}

}
