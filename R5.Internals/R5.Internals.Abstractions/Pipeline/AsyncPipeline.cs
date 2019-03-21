using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.Internals.Abstractions.Pipeline
{
	public abstract class AsyncPipeline<TContext>
	{
		private string _name { get; }

		public AsyncPipeline(string name)
		{
			_name = name;
		}

		public async Task ProcessAsync(TContext context)
		{
			OnPipelineProcessStart(context, _name);

			List<AsyncPipelineStage<TContext>> stages = GetStages();

			foreach(var stage in stages)
			{
				if (await stage.ShouldSkipAsync(context))
				{
					continue;
				}

				Guid stageId = OnStageProcessStart(context, stage.Name);

				ProcessStageResult result = await stage.ProcessAsync(context);

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

				OnStageProcessEnd(stageId, context, stage.Name);

				if (endProcessing)
				{
					break;
				}
			}

			//

			//AsyncPipelineStage<TContext> currentStage = _head;
			//while (currentStage != null)
			//{
			//	if (await currentStage.ShouldSkipAsync(context))
			//	{
			//		currentStage = currentStage.Next;
			//		continue;
			//	}

			//	Guid stageId = OnStageProcessStart(context, currentStage.Name);

			//	ProcessStageResult result = await currentStage.ProcessAsync(context);

			//	bool endProcessing = false;
			//	switch (result)
			//	{
			//		case Continue _:
			//			break;
			//		case End _:
			//			endProcessing = true;
			//			break;
			//		default:
			//			throw new ArgumentOutOfRangeException($"'{result.GetType().Name}' is an invalid process stage result type.");
			//	}

			//	OnStageProcessEnd(stageId, context, currentStage.Name);

			//	if (endProcessing)
			//	{
			//		break;
			//	}

			//	currentStage = currentStage.Next;
			//}

			OnPipelineProcessEnd(context, _name);
		}

		protected abstract List<AsyncPipelineStage<TContext>> GetStages();

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
