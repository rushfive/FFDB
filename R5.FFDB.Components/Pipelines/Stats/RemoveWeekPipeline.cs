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
	public class RemoveWeekPipeline : AsyncPipeline<RemoveWeekPipeline.Context>
	{
		private ILogger<RemoveWeekPipeline> _logger { get; }

		public RemoveWeekPipeline(
			AsyncPipelineStage<Context> head,
			ILogger<RemoveWeekPipeline> logger)
			: base(head, "Remove Stats for Week")
		{
			_logger = logger;
		}


		public class Context
		{
			public WeekInfo Week { get; set; }
		}

		protected override void OnPipelineProcessStart(Context context, string name)
		{
			_logger.LogInformation($"Starting pipeline to remove stats for week {context.Week}.");
		}

		protected override void OnPipelineProcessEnd(Context context, string name)
		{
			_logger.LogInformation($"Finished processing pipeline to remove stats for week {context.Week}.");
		}

		protected override void OnStageProcessStart(Context context, string name)
		{
			_logger.LogDebug($"Starting stage '{name}'.");
		}

		protected override void OnStageProcessEnd(Context context, string name)
		{
			_logger.LogInformation($"Finished processing stage '{name}'.");
		}

		public static RemoveWeekPipeline Create(IServiceProvider serviceProvider)
		{
			AsyncPipelineStage<Context> removeWeekStats = serviceProvider.ResolveInstance<Stages.RemoveWeekStats>();
			AsyncPipelineStage<Context> removeTeamGameStats = serviceProvider.ResolveInstance<Stages.RemoveTeamGameStats>();
			AsyncPipelineStage<Context> removeLog = serviceProvider.ResolveInstance<Stages.RemoveLog>();

			removeWeekStats
				.SetNext(removeTeamGameStats)
				.SetNext(removeLog);

			return serviceProvider.ResolveInstance<RemoveWeekPipeline>(removeWeekStats);
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
					await _dbProvider.GetContext().Stats.RemoveForWeekAsync(context.Week);
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
					await _dbProvider.GetContext().Team.RemoveGameStatsForWeekAsync(context.Week);
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
					await _dbProvider.GetContext().Log.RemoveForWeekAsync(context.Week);
					return ProcessResult.Continue;
				}
			}
		}
	}
}
