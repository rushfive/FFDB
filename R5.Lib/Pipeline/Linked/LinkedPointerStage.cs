using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.Lib.Pipeline.Linked
{
	// update: callback/fork stage is now just a dumb stage, that selects program path from callback
	// - remove processing logic from it
	

	// contains a pointer to next
	// RETHINK the API on this, returning the NEXT, rather than itself on SETNEXT results in
	// you having to safe a ref to the first/head stage BUT allows for better chaining afterwards
	public class LinkedPointerStage<TContext, TIn, TOut> : LinkedPipelineStage<TContext, TIn, TOut>
	{
		private Func<TContext, TIn, Task<ProcessStageResult<TOut>>> _processCallback { get; }
		
		// todo: see if theres a way to do this in a type-safe manner
		// issue: dont know the TInput type of the next stage
		private dynamic _next
		{
			get { return _next; }
			set
			{
				var type = value.GetType() as Type;
				Type[] genericArgs = type.GenericTypeArguments;

				bool validType = type.Name == "LinkedPipelineStage`2";
				bool validGenericArgs = genericArgs.Length == 3
					&& genericArgs[0] == typeof(TContext)
					&& genericArgs[1] == typeof(TIn);

				if (!validType || !validGenericArgs)
				{
					throw new InvalidOperationException("Next stage type is invalid. Must be a 'LinkedPointerStage'.");
				}

				_next = value;
			}
		}

		public LinkedPointerStage(Func<TContext, TIn, Task<ProcessStageResult<TOut>>> processCallback,
			string name = null)
		{
			_processCallback = processCallback ?? throw new ArgumentNullException(nameof(processCallback), "Process callback must be provided.");

			if (!string.IsNullOrWhiteSpace(name))
			{
				Name = name;
			}
		}

		public override bool HasNext()
		{
			return _next != null;
		}

		public override LinkedPipelineStage<TContext, TOut, TNextInput> GetNext<TNextInput>()
		{
			// check if we need this 'as' operator here
			return _next as LinkedPipelineStage<TContext, TOut, TNextInput>;
		}

		public override LinkedPipelineStage<TContext, TOut, TNextInput> SetNext<TNextInput>(Func<TContext, TOut, Task<ProcessStageResult<TNextInput>>> processCallback, string name = null)
		{
			return _next = new LinkedPointerStage<TContext, TOut, TNextInput>(processCallback, name);
		}

		public override LinkedPipelineStage<TContext, TOut, TNextInput> SetNext<TNextInput>(LinkedPipelineStage<TContext, TOut, TNextInput> stage)
		{
			return _next = stage ?? throw new ArgumentNullException(nameof(stage), "Next stage must be provided.");
		}
		
	}
}
