using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.Lib.Pipeline.Linked
{
	public class LinkedPipeline<TContext>
	{
		private LinkedPipelineStage<TContext> _head { get; }
	}

	public class LinkedPipelineStage<TContext>
	{
		private Func<TContext, Task<ProcessStageResult>> _process { get; }
		private LinkedPipelineStage<TContext> _next { get; set; }
		private Func<TContext, LinkedPipelineStage<TContext>> _getNext { get; set; }

		public LinkedPipelineStage(Func<TContext, Task<ProcessStageResult>> process)
		{
			_process = process ?? throw new ArgumentNullException(nameof(process), "Stage process callback must be provided.");
		}

		public Task<ProcessStageResult> ProcessAsync(TContext context)
		{
			return _process(context);
		}

		public LinkedPipelineStage<TContext> SetNext(LinkedPipelineStage<TContext> stage)
		{
			if (_getNext != null)
			{
				throw new InvalidOperationException("The getNext stage callback is already set. Cannot have both.");
			}

			_next = stage ?? throw new ArgumentNullException(nameof(stage), "Next stage must be provided.");
			return this;
		}

		public LinkedPipelineStage<TContext> SetGetNextCallback(Func<TContext, LinkedPipelineStage<TContext>> callback)
		{
			if (_next != null)
			{
				throw new InvalidOperationException("The next stage pointer is already set. Cannot have both.");
			}

			_getNext = callback ?? throw new ArgumentNullException(nameof(callback), "The getNext stage must be provided.");
			return this;
		}
	}

	public class LinkedStageChainBuilder<TContext>
	{
		private LinkedPipelineStage<TContext> _head { get; set; }
		private LinkedPipelineStage<TContext> _tail { get; set; }

		public LinkedStageChainBuilder<TContext> Next(LinkedPipelineStage<TContext> stage)
		{

		}

		public LinkedStageChainBuilder<TContext> Next(Func<TContext, Task<ProcessStageResult>> processCallback)
		{

		}
	}
}
