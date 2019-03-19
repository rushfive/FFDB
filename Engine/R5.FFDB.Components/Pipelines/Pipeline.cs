using Microsoft.Extensions.Logging;
using R5.Lib.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.Pipelines
{
	public abstract class Pipeline<TContext> : AsyncPipeline<TContext>
	{
		private IAppLogger _logger { get; }
		private string _pipelineIndent { get; set; }
		private string _stageIndent { get; set; }

		protected Pipeline(
			IAppLogger logger,
			AsyncPipelineStage<TContext> head,
			string name,
			int nestedDepth = 0)
			: base(head, name)
		{
			_logger = logger;

			SetLoggingIndents(nestedDepth);
		}

		private void SetLoggingIndents(int nestedDepth)
		{
			string pipeline = "";
			string stage = "  ";

			while (nestedDepth > 0)
			{
				pipeline += "    ";
				stage += "    ";

				nestedDepth--;
			}

			_pipelineIndent = pipeline;
			_stageIndent = stage;
		}

		protected override void OnPipelineProcessStart(TContext context, string name)
		{
			_logger.LogInformation($"{_pipelineIndent}[Pipeline - {name}] Starting.");
		}

		protected override void OnPipelineProcessEnd(TContext context, string name)
		{
			_logger.LogInformation($"{_pipelineIndent}[Pipeline - {name}] Ended.");
		}

		protected override void OnStageProcessStart(TContext context, string name)
		{
			_logger.LogDebug($"{_stageIndent}[Stage - {name}] Starting.");
		}

		protected override void OnStageProcessEnd(TContext context, string name)
		{
			_logger.LogInformation($"{_stageIndent}[Stage - {name}] Ended.");
		}


	}
}
