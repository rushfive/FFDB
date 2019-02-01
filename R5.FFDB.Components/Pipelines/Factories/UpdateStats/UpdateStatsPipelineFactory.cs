using Microsoft.Extensions.DependencyInjection;
using R5.FFDB.Components.Extensions;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.Core.Models;
using R5.Lib.Pipeline;
using R5.Lib.Pipeline.Linked;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Pipelines.Factories.UpdateStats
{
	public class UpdateStatsPipelineFactory
	{


		public UpdateStatsPipelineFactory()
		{

		}
	}



	// at lib pipeline level:
	// include hooks for logging, pre/post processing, etc
	// then, our derived implementation pipelines can customize behaviors AND
	// make it easier to add into DI
	public class UpdateStatsPipeline : LinkedPipeline<UpdateStatsPipeline.Context>
	{
		public UpdateStatsPipeline(LinkedPipelineStage<Context> head, WeekInfo week)
			: base($"Update Stats Pipeline ({week})", head)
		{
		}

		public class Context
		{
			public WeekInfo Week { get; set; }
		}

		public static UpdateStatsPipeline Create(IServiceProvider serviceProvider)
		{
			LinkedPointerStage<Context> head = serviceProvider.ResolveInstance<Stages.EndIfAlreadyUpdated>();


			throw new NotImplementedException();
		}

		public static class Stages
		{
			public class EndIfAlreadyUpdated : LinkedPointerStage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }

				public EndIfAlreadyUpdated(IDatabaseProvider dbProvider)
					: base("End if Week Already Processed")
				{
					_dbProvider = dbProvider;
				}

				public override Func<Context, Task<ProcessStageResult>> ProcessAsync => async context =>
				{
					IDatabaseContext dbContext = _dbProvider.GetContext();

					bool alreadyUpdated = await dbContext.Log.HasUpdatedWeekAsync(context.Week);
					if (alreadyUpdated)
					{
						return ProcessResult.End;
					}

					return ProcessResult.Continue;
				};
			}
		}
	}

}
