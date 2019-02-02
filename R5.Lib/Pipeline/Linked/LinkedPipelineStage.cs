using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.Lib.Pipeline.Linked
{
	public abstract class LinkedPipelineStage<TContext, TIn, TOut>
	{
		public virtual string Name { get; protected set; } = "Unnamed Stage";

		//private string _name { get; }

		//protected LinkedPipelineStage(string name)
		//{
		//	_name = name;
		//}

		//public abstract bool HasNext();
		//public abstract LinkedPipelineStage<TContext, TNextInput> GetNext<TNextInput>(TContext context);

		public abstract bool HasNext();
		public abstract LinkedPipelineStage<TContext, TOut, TNextInput> GetNext<TNextInput>();

		public abstract LinkedPipelineStage<TContext, TOut, TNextInput> SetNext<TNextInput>(Func<TContext, TOut, Task<ProcessStageResult<TNextInput>>> processCallback, string name = null);
		public abstract LinkedPipelineStage<TContext, TOut, TNextInput> SetNext<TNextInput>(LinkedPipelineStage<TContext, TOut, TNextInput> stage);

		//public abstract Task<ProcessStageResult<TNextInput>> ProcessAsync<TNextInput>(TContext context, TInput input);

		// virtual member with default, derived implementations can override, most likely calling a configurable callback func
		public virtual async Task<ProcessStageResult<TOutput>> ProcessAsync<TOutput>(TContext context, TIn input)
		{
			Console.WriteLine($"Ending processing because the stage hasn't defined process logic.");
			return ProcessResult.EndWith(default(TOutput));
		}

		
	}
}
