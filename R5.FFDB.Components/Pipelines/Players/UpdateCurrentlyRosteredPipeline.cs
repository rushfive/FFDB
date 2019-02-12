using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Extensions;
using R5.FFDB.Components.Extensions.Methods;
using R5.FFDB.Components.Pipelines.CommonStages;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Database.DbContext;
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


		public class Context : IFetchAddPlayersContext, IUpdatePlayersContext
		{
			public List<string> FetchAddNflIds { get; set; }
			public List<string> UpdateNflIds { get; set; }
		}

		public static UpdateCurrentlyRosteredPipeline Create(IServiceProvider sp)
		{
			AsyncPipelineStage<Context> groupByNewExisting = sp.Create<Stages.GroupByNewAndExisting>();
			AsyncPipelineStage<Context> fetchSavePlayers = sp.Create<FetchAddPlayersStage<Context>>();
			AsyncPipelineStage<Context> updatePlayers = sp.Create<UpdatePlayersStage<Context>>();

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
				//private RostersValue _rosters { get; }
				private IDatabaseProvider _dbProvider { get; }

				public GroupByNewAndExisting(
					ILogger<GroupByNewAndExisting> logger,
					//RostersValue rosters,
					IDatabaseProvider dbProvider)
					: base(logger, "Group by New and Existing")
				{
					//_rosters = rosters;
					_dbProvider = dbProvider;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					IDatabaseContext dbContext = _dbProvider.GetContext();

					HashSet<string> existingPlayers = (await dbContext.Player.GetAllAsync())
						.Select(p => p.NflId)
						.ToHashSet(StringComparer.OrdinalIgnoreCase);

					List<string> rosteredIds = null;// await _rosters.GetIdsAsync();

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
					
					context.FetchAddNflIds = newIds;
					context.UpdateNflIds = existingIds;

					return ProcessResult.Continue;
				}
			}
		}
	}

}
