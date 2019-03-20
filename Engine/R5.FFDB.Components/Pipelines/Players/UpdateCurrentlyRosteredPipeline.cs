using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.Dynamic.Rosters;
using R5.FFDB.Components.CoreData.Static.Players;
using R5.FFDB.Components.Extensions;
using R5.FFDB.Components.Pipelines.CommonStages;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.Internals.Abstractions.Pipeline;
using R5.Internals.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Pipelines.Players
{
	public class UpdateCurrentlyRosteredPipeline : Pipeline<UpdateCurrentlyRosteredPipeline.Context>
	{
		private IAppLogger _logger { get; }

		public UpdateCurrentlyRosteredPipeline(
			IAppLogger logger,
			IServiceProvider serviceProvider)
			: base(logger, serviceProvider, "Update Currently Rostered Players")
		{
			_logger = logger;
		}


		public class Context : IFetchPlayersContext
		{
			public List<string> FetchNflIds { get; set; }
			public List<string> UpdateNflIds { get; set; }
		}

		protected override List<Type> Stages => new List<Type>
		{
			typeof(Stage.GroupByNewAndExisting),
			typeof(FetchPlayersStage<Context>),
			typeof(Stage.UpdatePlayersStage)
		};

		public static class Stage
		{
			public class GroupByNewAndExisting : Stage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }
				private IRosterCache _rosterCache { get; }

				public GroupByNewAndExisting(
					IAppLogger logger,
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
					IAppLogger logger,
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
