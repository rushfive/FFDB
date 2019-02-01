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


	// contains a pointer to next
	public class LinkedPointerStage<TContext> : LinkedPipelineStage<TContext>
	{
		private LinkedPipelineStage<TContext> _next { get; set; }

		public LinkedPointerStage(Func<TContext, Task<ProcessStageResult>> process)
			: base(process)
		{
			
		}

		public LinkedPipelineStage<TContext> SetNext(LinkedPipelineStage<TContext> stage)
		{
			_next = stage ?? throw new ArgumentNullException(nameof(stage), "Next stage must be provided.");
			return this;
		}

		public override bool HasNext()
		{
			return _next != null;
		}

		public override LinkedPipelineStage<TContext> GetNext(TContext context)
		{
			if (_next == null)
			{
				throw new InvalidOperationException("Cannot get next stage because it isn't set.");
			}

			return _next;
		}
	}

	// contains callback to get the next
	public class LinkedCallbackStage<TContext> : LinkedPipelineStage<TContext>
	{
		private Func<TContext, LinkedPipelineStage<TContext>> _getNext { get; set; }

		public LinkedCallbackStage(Func<TContext, Task<ProcessStageResult>> process)
			: base(process)
		{
			
		}

		public LinkedPipelineStage<TContext> SetCallback(Func<TContext, LinkedPipelineStage<TContext>> callback)
		{
			_getNext = callback ?? throw new ArgumentNullException(nameof(callback), "The getNext stage must be provided.");
			return this;
		}

		public override bool HasNext()
		{
			return _getNext != null;
		}

		public override LinkedPipelineStage<TContext> GetNext(TContext context)
		{
			if (_getNext == null)
			{
				throw new InvalidOperationException("Cannot get next stage because its callback isn't set.");
			}

			return _getNext(context);
		}
	}

	public abstract class LinkedPipelineStage<TContext>
	{
		private Func<TContext, Task<ProcessStageResult>> _process { get; }

		protected LinkedPipelineStage(Func<TContext, Task<ProcessStageResult>> process)
		{
			_process = process ?? throw new ArgumentNullException(nameof(process), "Stage process callback must be provided.");
		}

		public abstract bool HasNext();
		public abstract LinkedPipelineStage<TContext> GetNext(TContext context);

		public Task<ProcessStageResult> ProcessAsync(TContext context)
		{
			return _process(context);
		}
	}

	

	// for now, should only use for stages with next pointers
	public class LinkedPointerChainBuilder<TContext>
	{
		private LinkedPointerStage<TContext> _head { get; set; }
		private LinkedPointerStage<TContext> _tail { get; set; }

		public LinkedPointerChainBuilder<TContext> Next(LinkedPointerStage<TContext> stage)
		{
			AddNext(stage);
			return this;
		}

		public (LinkedPointerStage<TContext> head, LinkedPointerStage<TContext> tail) GetChain()
		{
			return (_head, _tail);
		}

		private void AddNext(LinkedPointerStage<TContext> stage)
		{
			if (_head == null)
			{
				_head = _tail = stage;
				return;
			}

			_tail.SetNext(stage);

			// find new tail, the added stage may be a chain
			LinkedPipelineStage<TContext> curr = stage;
			while (curr.HasNext())
			{
				curr = curr.GetNext(default(TContext));
			}

			_tail = (LinkedPointerStage<TContext>)curr;
		}
	}
}
