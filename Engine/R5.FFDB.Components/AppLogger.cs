using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components
{
	public interface IAppLogger
	{
		void LogInformation(string message);
		void LogDebug(string message);
		void LogError(Exception exception, string message);
	}

	public class AppLogger : IAppLogger
	{
		private Serilog.ILogger _logger { get; }

		public AppLogger(Serilog.ILogger logger)
		{
			_logger = logger;
		}

		[MessageTemplateFormatMethod("messageTemplate")]
		public void LogInformation(string messageTemplate)
		{
			_logger?.Information(messageTemplate);
		}

		[MessageTemplateFormatMethod("messageTemplate")]
		public void LogDebug(string messageTemplate)
		{
			_logger?.Debug(messageTemplate);
		}

		[MessageTemplateFormatMethod("messageTemplate")]
		public void LogError(Exception exception, string messageTemplate)
		{
			_logger?.Error(exception, messageTemplate);
		}
	}

	public class CustomLogger : IAppLogger
	{
		private ILogger _logger { get; }

		public CustomLogger(ILogger logger)
		{
			_logger = logger;
		}

		public void LogInformation(string message)
		{
			_logger?.LogInformation(message);
		}

		public void LogDebug(string message)
		{
			_logger?.LogDebug(message);
		}

		public void LogError(Exception exception, string message)
		{
			_logger?.LogError(exception, message);
		}
	}
}
