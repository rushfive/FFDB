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
	public class UpdateAllPipeline : Pipeline<UpdateAllPipeline.Context>
	{
		private ILogger<UpdateAllPipeline> _logger { get; }

		public UpdateAllPipeline(
			ILogger<UpdateAllPipeline> logger,
			AsyncPipelineStage<Context> head)
			: base(logger, head, "Update All Existing Players")
		{
			_logger = logger;
		}


		public class Context : IUpdatePlayersContext
		{
			public List<string> UpdateNflIds { get; set; }
		}

		public static UpdateAllPipeline Create(IServiceProvider sp)
		{
			AsyncPipelineStage<Context> resolveAllExisting = sp.Create<Stages.ResolveAllExistingPlayers>();
			AsyncPipelineStage<Context> updatePlayers = sp.Create<UpdatePlayersStage<Context>>();

			AsyncPipelineStage<Context> chain = resolveAllExisting;
			chain.SetNext(updatePlayers);

			return sp.Create<UpdateAllPipeline>(chain);
		}

		public static class Stages
		{
			public class ResolveAllExistingPlayers : Stage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }

				public ResolveAllExistingPlayers(
					ILogger<ResolveAllExistingPlayers> logger,
					IDatabaseProvider dbProvider)
					: base(logger, "Resolve All Existing Players")
				{
					_dbProvider = dbProvider;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					IDatabaseContext dbContext = _dbProvider.GetContext();

					List<string> existingIds = (await dbContext.Player.GetAllAsync())
						.Select(p => p.NflId)
						.ToList();

					context.UpdateNflIds = existingIds;

					return ProcessResult.Continue;
				}
			}
		}
	}
}
