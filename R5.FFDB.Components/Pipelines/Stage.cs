using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using R5.Lib.Pipeline;
using System;

namespace R5.FFDB.Components.Pipelines
{
	public abstract class Stage<TContext> : AsyncPipelineStage<TContext>
	{
		private ILogger<Stage<TContext>> _logger { get; }
		private string _indent { get; set; }

		protected Stage(
			ILogger<Stage<TContext>> logger,
			string name,
			int nestedDepth = 0) 
			: base(name)
		{
			_logger = logger;

			SetLoggingIndents(nestedDepth);
		}

		private void SetLoggingIndents(int nestedDepth)
		{
			string indent = "  ";

			while (nestedDepth > 0)
			{
				indent += "    ";
				nestedDepth--;
			}

			_indent = indent;
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

		protected void LogWarning(string message)
		{
			_logger.LogWarning(NamePrepended(message));
		}

		private string NamePrepended(string message) => $"{_indent}[Stage - {Name}] {message}";
	}
}
