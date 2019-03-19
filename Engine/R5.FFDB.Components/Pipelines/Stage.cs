using R5.Internals.Abstractions.Pipeline;

namespace R5.FFDB.Components.Pipelines
{
	public abstract class Stage<TContext> : AsyncPipelineStage<TContext>
	{
		private IAppLogger _logger { get; }

		protected Stage(
			IAppLogger logger,
			string name) 
			: base(name)
		{
			_logger = logger;
		}

		protected void LogInformation(string message)
		{
			_logger.LogInformation(message);
		}

		protected void LogDebug(string message)
		{
			_logger.LogDebug(message);
		}

		protected void LogWarning(string message)
		{
			_logger.LogWarning(message);
		}
	}
}
