using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.Lib.Pipeline.Linked
{
	public abstract class LinkedPipelineStage<TContext>
	{
		private string _name { get; }

		protected LinkedPipelineStage(string name)
		{
			_name = name;
		}

		public abstract bool HasNext();
		public abstract LinkedPipelineStage<TContext> GetNext(TContext context);
		public abstract LinkedPipelineStage<TContext> SetNext(string name, Func<TContext, Task<ProcessStageResult>> processCallback);
		public abstract LinkedPipelineStage<TContext> SetNext(LinkedPipelineStage<TContext> stage);

		public virtual Func<TContext, Task<ProcessStageResult>> ProcessAsync { get; set; } = context =>
		{
			Console.WriteLine($"Ending processing because the stage hasn't defined process logic.");
			return Task.FromResult(ProcessResult.End);
		};

		//public virtual Task<ProcessStageResult> ProcessAsync(TContext context)
		//{
		//	Console.WriteLine($"Ending processing because stage '{_name}' hasn't defined process logic.");
		//	return Task.FromResult<ProcessStageResult>(ProcessResult.End);
		//}

		//public Task<ProcessStageResult> ProcessAsync(TContext context)
		//{
		//	return _process(context);
		//}

		public string GetName()
		{
			return _name;
		}
	}
}
