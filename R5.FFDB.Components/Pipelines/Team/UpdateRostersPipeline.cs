using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.Rosters.Values;
using R5.FFDB.Components.Extensions;
using R5.FFDB.Components.Pipelines.CommonStages;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.Core.Entities;
using R5.Lib.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Pipelines.Team
{
	public class UpdateRostersPipeline : Pipeline<UpdateRostersPipeline.Context>
	{
		private ILogger<UpdateRostersPipeline> _logger { get; }

		public UpdateRostersPipeline(
			ILogger<UpdateRostersPipeline> logger,
			AsyncPipelineStage<Context> head)
			: base(logger, head, "Update Rosters")
		{
			_logger = logger;
		}


		public class Context : IRosteredPlayersContext, IFetchAddPlayersContext
		{
			public List<string> RosteredNflIds { get; set; }
			public List<string> FetchAddNflIds { get; set; }
		}

		

		public static UpdateRostersPipeline Create(IServiceProvider sp)
		{
			AsyncPipelineStage<Context> getIds = sp.Create<GetCurrentRosteredPlayerIds<Context>>();
			AsyncPipelineStage<Context> fetchSavePlayers = sp.Create<FetchAddPlayersStage<Context>>();
			AsyncPipelineStage<Context> update = sp.Create<Stages.Update>();

			AsyncPipelineStage<Context> chain = getIds;
			chain
				.SetNext(fetchSavePlayers)
				.SetNext(update);

			return sp.Create<UpdateRostersPipeline>(chain);
		}

		public static class Stages
		{



			//
			public class Update : AsyncPipelineStage<Context>
			{
				private RostersValue _rosters { get; }
				private IDatabaseProvider _dbProvider { get; }

				public Update(
					RostersValue rosters,
					IDatabaseProvider dbProvider)
					: base("Update Rosters")
				{
					_rosters = rosters;
					_dbProvider = dbProvider;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					IDatabaseContext dbContext = _dbProvider.GetContext();

					List<Roster> rosters = await _rosters.GetAsync();

					await dbContext.Team.UpdateRostersAsync(rosters);

					return ProcessResult.Continue;
				}
			}
		}
	}

}
