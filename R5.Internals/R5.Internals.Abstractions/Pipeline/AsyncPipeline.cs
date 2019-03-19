using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.Internals.Abstractions.Pipeline
{
	public class AsyncPipeline<TContext>
	{
		private AsyncPipelineStage<TContext> _head { get; }
		private string _name { get; }

		public AsyncPipeline(
			AsyncPipelineStage<TContext> head,
			string name)
		{
			_head = head;
			_name = name;
		}

		public async Task ProcessAsync(TContext context)
		{
			OnPipelineProcessStart(context, _name);

			AsyncPipelineStage<TContext> currentStage = _head;
			while (currentStage != null)
			{
				if (await currentStage.ShouldSkipAsync(context))
				{
					currentStage = currentStage.Next;
					continue;
				}

				Guid stageId = OnStageProcessStart(context, currentStage.Name);

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

				OnStageProcessEnd(stageId, context, currentStage.Name);

				if (endProcessing)
				{
					break;
				}

				currentStage = currentStage.Next;
			}

			OnPipelineProcessEnd(context, _name);
		}

		protected virtual void OnPipelineProcessStart(TContext context, string name) { }

		protected virtual void OnPipelineProcessEnd(TContext context, string name) { }

		protected virtual Guid OnStageProcessStart(TContext context, string name) { return default; }

		protected virtual void OnStageProcessEnd(Guid stageId, TContext context, string name) { }

		public override string ToString()
		{
			return $"{_name} [pipeline]";
		}
	}
}
