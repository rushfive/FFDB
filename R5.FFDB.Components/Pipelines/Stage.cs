using Microsoft.Extensions.Logging;
using R5.Lib.Pipeline;

namespace R5.FFDB.Components.Pipelines
{
	public abstract class Stage<TContext> : AsyncPipelineStage<TContext>
	{
		private ILogger<Stage<TContext>> _logger { get; }

		protected Stage(
			ILogger<Stage<TContext>> logger,
			string name) 
			: base(name)
		{
			_logger = logger;
		}

		protected void LogInformation(string message)
		{
			_logger.LogInformation(NamePrepended(message));
		}

		protected void LogDebug(string message)
		{
			_logger.LogDebug(NamePrepended(message));
		}

		protected void LogTrace(string message)
		{
			_logger.LogTrace(NamePrepended(message));
		}

		private string NamePrepended(string message) => $"[{Name}] message";
	}
}
