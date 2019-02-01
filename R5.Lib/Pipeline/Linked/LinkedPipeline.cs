using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.Lib.Pipeline.Linked
{
	public class LinkedPipeline<TContext>
	{
		private LinkedPipelineStage<TContext> _head { get; }
		private TContext _context { get; }

		public LinkedPipeline(
			LinkedPipelineStage<TContext> head,
			TContext context)
		{
			_head = head ?? throw new ArgumentNullException(nameof(head), "At least one stage must be provided.");
			_context = context;
		}

		public async Task ProcessAsync()
		{
			LinkedPipelineStage<TContext> currentStage = _head;
			while (currentStage != null)
			{
				ProcessStageResult result = await currentStage.ProcessAsync(_context);

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

				currentStage = currentStage.GetNext(_context);
			}
		}
	}


	// contains a pointer to next
	// RETHINK the API on this, returning the NEXT, rather than itself on SETNEXT results in
	// you having to safe a ref to the first/head stage BUT allows for better chaining afterwards
	public class LinkedPointerStage<TContext> : LinkedPipelineStage<TContext>
	{
		private LinkedPipelineStage<TContext> _next { get; set; }

		public LinkedPointerStage(Func<TContext, Task<ProcessStageResult>> process)
			: base(process)
		{
			
		}

		// returns the next stage for easy fluent chaining
		public override LinkedPipelineStage<TContext> SetNext(LinkedPipelineStage<TContext> stage)
		{
			_next = stage ?? throw new ArgumentNullException(nameof(stage), "Next stage must be provided.");
			return _next;
		}

		// returns the next stage for easy fluent chaining
		public override LinkedPipelineStage<TContext> SetNext(Func<TContext, Task<ProcessStageResult>> processCallback)
		{
			if (processCallback == null)
			{
				throw new ArgumentNullException(nameof(processCallback), "Process callback must be provided.");
			}

			_next = new LinkedPointerStage<TContext>(processCallback);
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

		public override LinkedPipelineStage<TContext> SetNext(LinkedPipelineStage<TContext> stage)
		{
			throw new NotImplementedException();
		}

		public override LinkedPipelineStage<TContext> SetNext(Func<TContext, Task<ProcessStageResult>> processCallback)
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

	public abstract class LinkedPipelineStage<TContext>
	{
		private Func<TContext, Task<ProcessStageResult>> _process { get; }

		protected LinkedPipelineStage(Func<TContext, Task<ProcessStageResult>> process)
		{
			_process = process ?? throw new ArgumentNullException(nameof(process), "Stage process callback must be provided.");
		}

		public abstract bool HasNext();
		public abstract LinkedPipelineStage<TContext> GetNext(TContext context);
		public abstract LinkedPipelineStage<TContext> SetNext(Func<TContext, Task<ProcessStageResult>> processCallback);
		public abstract LinkedPipelineStage<TContext> SetNext(LinkedPipelineStage<TContext> stage);

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

		public LinkedPointerChainBuilder<TContext> Next(Func<TContext, Task<ProcessStageResult>> processCallback)
		{
			var stage = new LinkedPointerStage<TContext>(processCallback);

			AddNext(stage);
			return this;
		}

		public (LinkedPointerStage<TContext> head, LinkedPointerStage<TContext> tail) Build()
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
