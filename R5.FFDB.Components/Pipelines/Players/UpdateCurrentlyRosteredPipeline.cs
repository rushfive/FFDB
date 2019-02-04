//using Microsoft.Extensions.Logging;
//using R5.FFDB.Components.Extensions;
//using R5.FFDB.Components.Pipelines.CommonStages;
//using R5.Lib.Pipeline;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace R5.FFDB.Components.Pipelines.Players
//{
//	public class UpdateCurrentlyRosteredPipeline : Pipeline<UpdateCurrentlyRosteredPipeline.Context>
//	{
//		private ILogger<UpdateCurrentlyRosteredPipeline> _logger { get; }

//		public UpdateCurrentlyRosteredPipeline(
//			ILogger<UpdateCurrentlyRosteredPipeline> logger,
//			AsyncPipelineStage<Context> head)
//			: base(logger, head, "Update Currently Rostered Players")
//		{
//			_logger = logger;
//		}


//		public class Context : IFetchAddPlayersContext
//		{
//			public List<string> FetchAddNflIds { get; set; }
//			public List<string> CurrentNflIds { get; set; }
//		}

//		public static UpdateCurrentlyRosteredPipeline Create(IServiceProvider sp)
//		{
//			AsyncPipelineStage<Context> getIds = sp.Create<GetCurrentRosteredPlayerIds<Context>>();

//			// get ids that DONT currently exist in db
//			// fetch and save those first
//			// then, update the rest



//			AsyncPipelineStage<Context> fetchSavePlayers = sp.Create<FetchAddPlayersStage<Context>>();
//			AsyncPipelineStage<Context> update = sp.Create<Stages.FetchSaveNewPlayers>();

//			AsyncPipelineStage<Context> chain = getIds;
//			chain
//				.SetNext(fetchSavePlayers)
//				.SetNext(update);

//			return sp.Create<UpdateCurrentlyRosteredPipeline>(chain);
//		}

//		public static class Stages
//		{
			

//			public class FetchSaveNewPlayers : AsyncPipelineStage<Context>
//			{
//				private RostersValue _rosters { get; }
//				private IDatabaseProvider _dbProvider { get; }

//				public FetchSaveNewPlayers(
//					RostersValue rosters,
//					IDatabaseProvider dbProvider)
//					: base("Update Rosters")
//				{
//					_rosters = rosters;
//					_dbProvider = dbProvider;
//				}

//				public override async Task<ProcessStageResult> ProcessAsync(Context context)
//				{
//					IDatabaseContext dbContext = _dbProvider.GetContext();

//					List<Roster> rosters = await _rosters.GetAsync();

//					await dbContext.Team.UpdateRostersAsync(rosters);

//					return ProcessResult.Continue;
//				}
//			}
//		}
//	}

//}
