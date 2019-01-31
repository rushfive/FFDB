using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.Lib.Pipeline.Sequential
{
	public class SequentialPipeline<TContext>
	{
		private List<SequentialPipelineStage<TContext>> _pipeline { get; } = new List<SequentialPipelineStage<TContext>>();

		public async Task ProcessAsync(TContext context)
		{
			foreach (var stage in _pipeline)
			{
				ProcessStageResult result = await stage.ProcessAsync(context);

				bool endProcessing = false;
				switch (result)
				{
					case Continue _:
						break;
					case End _:
						endProcessing = true;
						break;
					default:
						throw new ArgumentOutOfRangeException($"'{result.GetType().Name}' is an invalid process stage result type.");
				}

				if (endProcessing)
				{
					break;
				}
			}
		}

		public SequentialPipeline<TContext> Next(SequentialPipelineStage<TContext> stage)
		{
			_pipeline.Add(stage);
			return this;
		}
	}
}
