using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.Lib.Pipeline.Linked
{
	// contains callback to get the next
	public class LinkedCallbackStage<TContext, TIn> : LinkedPipelineStage<TContext, TIn, Task>
	{
		// todo: see if theres a way to do this in a type-safe manner
		// issue: dont know the TInput type of the next stage
		private dynamic _getNext
		{
			get { return _getNext; }
			set
			{
				var type = value.GetType() as Type;
				Type[] genericArgs = type.GenericTypeArguments;

				bool validType = type.Name == "Func`3";
				bool validGenericArgs = genericArgs.Length == 3
					&& genericArgs[0] == typeof(TContext)
					&& genericArgs[1] == typeof(TIn)
					&& genericArgs[2].Name == "LinkedPipelineStage`3";
				bool 

				if (!validType || !validGenericArgs)
				{
					throw new InvalidOperationException("Next stage type is invalid. Must be a 'LinkedPointerStage'.");
				}

				_next = value;
			}
		}


		private Func<TContext, TIn, LinkedPipelineStage<TContext, Task, dynamic>> _getNext { get; set; }

		public LinkedCallbackStage(Func<TContext, TIn, dynamic> processCallback,
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
			throw new NotImplementedException();
		}

		public override LinkedPipelineStage<TContext, Task, TNextInput> GetNext<TNextInput>()
		{
			throw new NotImplementedException();
		}

		

		public override LinkedPipelineStage<TContext, Task, TNextInput> SetNext<TNextInput>(Func<TContext, Task, Task<ProcessStageResult<TNextInput>>> processCallback, string name = null)
		{
			throw new NotImplementedException();
		}

		public override LinkedPipelineStage<TContext, Task, TNextInput> SetNext<TNextInput>(LinkedPipelineStage<TContext, Task, TNextInput> stage)
		{
			throw new NotImplementedException();
		}







		//private Func<TContext, LinkedPipelineStage<TContext>> _getNext { get; set; }

		//public LinkedCallbackStage(
		//	string name,
		//	Func<TContext, Task<ProcessStageResult>> process)
		//	: base(name)
		//{
		//	if (process != null)
		//	{
		//		ProcessAsync = process;
		//	}
		//}

		//public LinkedPipelineStage<TContext> SetCallback(Func<TContext, LinkedPipelineStage<TContext>> callback)
		//{
		//	_getNext = callback ?? throw new ArgumentNullException(nameof(callback), "The getNext stage must be provided.");
		//	return this;
		//}

		//public override LinkedPipelineStage<TContext> SetNext(LinkedPipelineStage<TContext> stage)
		//{
		//	throw new NotImplementedException();
		//}

		//public override LinkedPipelineStage<TContext> SetNext(string name,
		//	Func<TContext, Task<ProcessStageResult>> processCallback)
		//{
		//	throw new NotImplementedException();
		//}

		//public override bool HasNext()
		//{
		//	return _getNext != null;
		//}

		//public override LinkedPipelineStage<TContext> GetNext(TContext context)
		//{
		//	return _getNext(context);
		//}

	}
}
