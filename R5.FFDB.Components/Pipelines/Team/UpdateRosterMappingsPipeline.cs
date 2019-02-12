using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Extensions;
using R5.FFDB.Components.Extensions.Methods;
using R5.FFDB.Components.Pipelines.CommonStages;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.Core.Entities;
using R5.Lib.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Pipelines.Team
{
	public class UpdateRosterMappingsPipeline : Pipeline<UpdateRosterMappingsPipeline.Context>
	{
		private ILogger<UpdateRosterMappingsPipeline> _logger { get; }

		public UpdateRosterMappingsPipeline(
			ILogger<UpdateRosterMappingsPipeline> logger,
			AsyncPipelineStage<Context> head)
			: base(logger, head, "Update Rosters")
		{
			_logger = logger;
		}


		public class Context : IFetchAddPlayersContext
		{
			public List<string> FetchAddNflIds { get; set; }
		}

		

		public static UpdateRosterMappingsPipeline Create(IServiceProvider sp)
		{
			AsyncPipelineStage<Context> resolveNewRosteredPlayers = sp.Create<Stages.ResolveNewRosteredPlayers>();
			AsyncPipelineStage<Context> fetchSavePlayers = sp.Create<FetchAddPlayersStage<Context>>();
			AsyncPipelineStage<Context> update = sp.Create<Stages.Update>();

			AsyncPipelineStage<Context> chain = resolveNewRosteredPlayers;
			chain
				.SetNext(fetchSavePlayers)
				.SetNext(update);

			return sp.Create<UpdateRosterMappingsPipeline>(chain);
		}

		public static class Stages
		{
			public class ResolveNewRosteredPlayers : Stage<Context>
			{
				//private RostersValue _rosters { get; }
				private IDatabaseProvider _dbProvider { get; }

				public ResolveNewRosteredPlayers(
					ILogger<ResolveNewRosteredPlayers> logger,
					//RostersValue rosters,
					IDatabaseProvider dbProvider) 
					: base(logger, "Resolve New Rostered Players")
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

					List<string> newIds = null;// (await _rosters.GetIdsAsync())
						//.Where(id => !existingPlayers.Contains(id))
						//.ToList();

					context.FetchAddNflIds = newIds;

					return ProcessResult.Continue;
				}
			}
			
			public class Update : Stage<Context>
			{
				//private RostersValue _rosters { get; }
				private IDatabaseProvider _dbProvider { get; }

				public Update(
					ILogger<Update> logger,
					//RostersValue rosters,
					IDatabaseProvider dbProvider)
					: base(logger, "Update Rosters")
				{
					//_rosters = rosters;
					_dbProvider = dbProvider;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					IDatabaseContext dbContext = _dbProvider.GetContext();

					List<Roster> rosters = null;// await _rosters.GetAsync();

					await dbContext.Team.UpdateRosterMappingsAsync(rosters);

					return ProcessResult.Continue;
				}
			}
		}
	}

}
