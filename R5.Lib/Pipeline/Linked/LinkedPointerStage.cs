using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.Lib.Pipeline.Linked
{
	// contains a pointer to next
	// RETHINK the API on this, returning the NEXT, rather than itself on SETNEXT results in
	// you having to safe a ref to the first/head stage BUT allows for better chaining afterwards
	public class LinkedPointerStage<TContext> : LinkedPipelineStage<TContext>
	{
		private LinkedPipelineStage<TContext> _next { get; set; }

		public LinkedPointerStage(
			string name,
			Func<TContext, Task<ProcessStageResult>> process = null)
			: base(name)
		{
			if (process != null)
			{
				ProcessAsync = process;
			}
		}

		// returns the next stage for easy fluent chaining
		public override LinkedPipelineStage<TContext> SetNext(LinkedPipelineStage<TContext> stage)
		{
			_next = stage ?? throw new ArgumentNullException(nameof(stage), "Next stage must be provided.");
			return _next;
		}

		// returns the next stage for easy fluent chaining
		public override LinkedPipelineStage<TContext> SetNext(string name,
			Func<TContext, Task<ProcessStageResult>> processCallback)
		{
			if (processCallback == null)
			{
				throw new ArgumentNullException(nameof(processCallback), "Process callback must be provided.");
			}

			_next = new LinkedPointerStage<TContext>(name, processCallback);
			return _next;
		}

		public override bool HasNext()
		{
			return _next != null;
		}

		public override LinkedPipelineStage<TContext> GetNext(TContext context)
		{
			return _next;
		}
	}
}
