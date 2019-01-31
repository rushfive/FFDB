using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.Lib.Pipeline.Sequential
{
	public static class SequentialPipelineExtensions
	{
		public static SequentialPipeline<TContext> Next<TContext>(this SequentialPipeline<TContext> pipeline,
			Func<TContext, Task<ProcessStageResult>> processCallback)
		{
			var stage = new SequentialPipelineStage<TContext>(processCallback);
			return pipeline.Next(stage);
		}
	}
}
