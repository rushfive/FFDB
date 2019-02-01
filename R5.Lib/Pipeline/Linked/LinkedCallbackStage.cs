using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.Lib.Pipeline.Linked
{
	// contains callback to get the next
	public class LinkedCallbackStage<TContext> : LinkedPipelineStage<TContext>
	{
		private Func<TContext, LinkedPipelineStage<TContext>> _getNext { get; set; }

		public LinkedCallbackStage(
			string name,
			Func<TContext, Task<ProcessStageResult>> process)
			: base(name)
		{
			if (process != null)
			{
				ProcessAsync = process;
			}
		}

		public LinkedPipelineStage<TContext> SetCallback(Func<TContext, LinkedPipelineStage<TContext>> callback)
		{
			_getNext = callback ?? throw new ArgumentNullException(nameof(callback), "The getNext stage must be provided.");
			return this;
		}

		public override LinkedPipelineStage<TContext> SetNext(LinkedPipelineStage<TContext> stage)
		{
			throw new NotImplementedException();
		}

		public override LinkedPipelineStage<TContext> SetNext(string name,
			Func<TContext, Task<ProcessStageResult>> processCallback)
		{
			throw new NotImplementedException();
		}

		public override bool HasNext()
		{
			return _getNext != null;
		}

		public override LinkedPipelineStage<TContext> GetNext(TContext context)
		{
			return _getNext(context);
		}
	}
}
