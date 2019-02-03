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

		public RemoveWeekPipeline(AsyncPipelineStage<Context> head, WeekInfo week)
			: base(head, $"Remove Stats for Week Pipeline ({week})")
		{

		}


		public class Context
		{
			public WeekInfo Week { get; set; }
		}

		public static RemoveWeekPipeline Create(IServiceProvider serviceProvider)
		{
			AsyncPipelineStage<Context> head = serviceProvider.ResolveInstance<Stages.RemoveWeekStats>();


			throw new NotImplementedException();
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
		}
	}
}
