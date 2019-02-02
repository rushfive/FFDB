using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.Lib.Pipeline.Linked
{
	// todo: implement ienumerable
	public class LinkedPipeline<TContext>
	{
		private string _name { get; }
		private LinkedPipelineStage<TContext> _head { get; }
		

		public LinkedPipeline(
			string name,
			LinkedPipelineStage<TContext> head)
		{
			_name = name;
			_head = head ?? throw new ArgumentNullException(nameof(head), "At least one stage must be provided.");
		}

		public async Task ProcessAsync(TContext context)
		{
			OnPipelineProcessStart(_name);

			LinkedPipelineStage<TContext> currentStage = _head;
			while (currentStage != null)
			{
				OnStageProcessStart(currentStage.GetName());

				ProcessStageResult result = await currentStage.ProcessAsync(context);

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

				OnStageProcessEnd(currentStage.GetName());

				if (endProcessing)
				{
					break;
				}

				currentStage = currentStage.GetNext(context);
			}

			OnPipelineProcessEnd(_name);
		}

		protected virtual void OnPipelineProcessStart(string name) { }

		protected virtual void OnPipelineProcessEnd(string name) { }

		protected virtual void OnStageProcessStart(string name) { }

		protected virtual void OnStageProcessEnd(string name) { }

		public override string ToString()
		{
			return $"{_name} [pipeline]";
		}
	}


	

	

	

	

	// for now, should only use for stages with next pointers
	//public class LinkedPointerChainBuilder<TContext>
	//{
	//	private LinkedPointerStage<TContext> _head { get; set; }
	//	private LinkedPointerStage<TContext> _tail { get; set; }

	//	public LinkedPointerChainBuilder<TContext> Next(LinkedPointerStage<TContext> stage)
	//	{
	//		AddNext(stage);
	//		return this;
	//	}

	//	public LinkedPointerChainBuilder<TContext> Next(Func<TContext, Task<ProcessStageResult>> processCallback)
	//	{
	//		var stage = new LinkedPointerStage<TContext>(processCallback);

	//		AddNext(stage);
	//		return this;
	//	}

	//	public (LinkedPointerStage<TContext> head, LinkedPointerStage<TContext> tail) Build()
	//	{
	//		return (_head, _tail);
	//	}

	//	private void AddNext(LinkedPointerStage<TContext> stage)
	//	{
	//		if (_head == null)
	//		{
	//			_head = _tail = stage;
	//			return;
	//		}

	//		_tail.SetNext(stage);

	//		// find new tail, the added stage may be a chain
	//		LinkedPipelineStage<TContext> curr = stage;
	//		while (curr.HasNext())
	//		{
	//			curr = curr.GetNext(default(TContext));
	//		}

	//		_tail = (LinkedPointerStage<TContext>)curr;
	//	}
	//}
}
