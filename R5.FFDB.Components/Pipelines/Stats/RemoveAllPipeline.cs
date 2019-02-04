using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Extensions;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Models;
using R5.Lib.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Pipelines.Stats
{
	public class RemoveAllPipeline : Pipeline<RemoveAllPipeline.Context>
	{
		private ILogger<RemoveAllPipeline> _logger { get; }

		public RemoveAllPipeline(
			ILogger<RemoveAllPipeline> logger,
			AsyncPipelineStage<Context> head)
			: base(logger, head, "Remove All Stats")
		{
			_logger = logger;
		}


		public class Context { }

		public static RemoveWeekPipeline Create(IServiceProvider sp)
		{
			AsyncPipelineStage<Context> removeWeekStats = sp.Create<Stages.RemoveWeekStats>();
			AsyncPipelineStage<Context> removeTeamGameStats = sp.Create<Stages.RemoveTeamGameStats>();
			AsyncPipelineStage<Context> removeLog = sp.Create<Stages.RemoveLog>();

			AsyncPipelineStage<Context> chain = removeWeekStats;
			chain
				.SetNext(removeTeamGameStats)
				.SetNext(removeLog);

			return sp.Create<RemoveWeekPipeline>(chain);
		}

		public static class Stages
		{
			public class RemoveWeekStats : AsyncPipelineStage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }

				public RemoveWeekStats(IDatabaseProvider dbProvider)
					: base("Remove Week Stats")
				{
					_dbProvider = dbProvider;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					await _dbProvider.GetContext().Stats.RemoveAllAsync();
					return ProcessResult.Continue;
				}
			}

			public class RemoveTeamGameStats : AsyncPipelineStage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }

				public RemoveTeamGameStats(IDatabaseProvider dbProvider)
					: base("Remove Game Stats")
				{
					_dbProvider = dbProvider;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					await _dbProvider.GetContext().Team.RemoveAllGameStatsAsync();
					return ProcessResult.Continue;
				}
			}

			public class RemoveLog : AsyncPipelineStage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }

				public RemoveLog(IDatabaseProvider dbProvider)
					: base("Remove Log")
				{
					_dbProvider = dbProvider;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					await _dbProvider.GetContext().Log.RemoveAllAsync();
					return ProcessResult.Continue;
				}
			}
		}
	}
}
