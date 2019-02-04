using Microsoft.Extensions.Logging;
using R5.Lib.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.Pipelines
{
	public abstract class Pipeline<TContext> : AsyncPipeline<TContext>
	{
		private ILogger<Pipeline<TContext>> _logger { get; }

		protected Pipeline(
			ILogger<Pipeline<TContext>> logger,
			AsyncPipelineStage<TContext> head,
			string name)
			: base(head, name)
		{
			_logger = logger;
		}

		protected override void OnPipelineProcessStart(TContext context, string name)
		{
			_logger.LogInformation($"Starting pipeline '{name}'.");
		}

		protected override void OnPipelineProcessEnd(TContext context, string name)
		{
			_logger.LogInformation($"Finished processing pipeline '{name}'.");
		}

		protected override void OnStageProcessStart(TContext context, string name)
		{
			_logger.LogDebug($"Starting stage '{name}'.");
		}

		protected override void OnStageProcessEnd(TContext context, string name)
		{
			_logger.LogInformation($"Finished processing stage '{name}'.");
		}
	}
}
