using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.Lib.Pipeline.Sequential
{
	public class SequentialPipelineStage<TContext>
	{
		private Func<TContext, Task<ProcessStageResult>> _process { get; }

		public SequentialPipelineStage(Func<TContext, Task<ProcessStageResult>> process)
		{
			_process = process ?? throw new ArgumentNullException(nameof(process));
		}

		public Task<ProcessStageResult> ProcessAsync(TContext context) => _process(context);
	}
}
