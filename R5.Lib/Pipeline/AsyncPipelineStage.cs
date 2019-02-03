using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.Lib.Pipeline
{
	public class AsyncPipelineStage<TContext>
	{
		public string Name { get; }
		public AsyncPipelineStage<TContext> Next { get; private set; }

		public AsyncPipelineStage(string name)
		{
			Name = name;
		}

		public virtual Task<bool> ShouldSkipAsync(TContext context)
		{
			return Task.FromResult(false);
		}

		public virtual Func<TContext, Task<ProcessStageResult>> OnProcessAsync { get; set; }

		public virtual async Task<ProcessStageResult> ProcessAsync(TContext context)
		{
			return OnProcessAsync != null
				? await OnProcessAsync(context)
				: ProcessResult.End;
		}

		public AsyncPipelineStage<TContext> SetNext(AsyncPipelineStage<TContext> nextStage)
		{
			Next = nextStage;
			return Next;
		}

		public AsyncPipelineStage<TContext> SetNext(string name,
			Func<TContext, Task<ProcessStageResult>> onProcessCallback)
		{
			Next = new AsyncPipelineStage<TContext>(name)
			{
				OnProcessAsync = onProcessCallback
			};

			return Next;
		}
	}
}
