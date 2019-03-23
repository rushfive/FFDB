using Microsoft.Extensions.Logging;
using R5.FFDB.Core;
using Serilog.Core;
using System;

namespace R5.FFDB.Components
{
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
		public void LogInformation<T>(string messageTemplate, T propertyValue)
		{
			_logger?.Information(messageTemplate, propertyValue);
		}

		[MessageTemplateFormatMethod("messageTemplate")]
		public void LogDebug(string messageTemplate)
		{
			_logger?.Debug(messageTemplate);
		}

		[MessageTemplateFormatMethod("messageTemplate")]
		public void LogDebug<T>(string messageTemplate, T propertyValue)
		{
			_logger?.Debug(messageTemplate, propertyValue);
		}

		[MessageTemplateFormatMethod("messageTemplate")]
		public void LogTrace(string messageTemplate)
		{
			_logger?.Verbose(messageTemplate);
		}

		[MessageTemplateFormatMethod("messageTemplate")]
		public void LogTrace<T>(string messageTemplate, T propertyValue)
		{
			_logger?.Verbose(messageTemplate, propertyValue);
		}

		[MessageTemplateFormatMethod("messageTemplate")]
		public void LogError(Exception exception, string messageTemplate)
		{
			_logger?.Error(exception, messageTemplate);
		}

		[MessageTemplateFormatMethod("messageTemplate")]
		public void LogError<T>(Exception exception, string messageTemplate, T propertyValue)
		{
			_logger?.Error(exception, messageTemplate, propertyValue);
		}

		[MessageTemplateFormatMethod("messageTemplate")]
		public void LogWarning(string messageTemplate)
		{
			_logger?.Warning(messageTemplate);
		}

		[MessageTemplateFormatMethod("messageTemplate")]
		public void LogWarning<T>(string messageTemplate, T propertyValue)
		{
			_logger?.Warning(messageTemplate, propertyValue);
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

		public void LogInformation<T>(string message, T propertyValue)
		{
			_logger?.LogInformation(message, propertyValue);
		}

		public void LogDebug(string message)
		{
			_logger?.LogDebug(message);
		}

		public void LogDebug<T>(string message, T propertyValue)
		{
			_logger?.LogDebug(message, propertyValue);
		}

		public void LogTrace(string message)
		{
			_logger?.LogTrace(message);
		}

		public void LogTrace<T>(string message, T propertyValue)
		{
			_logger?.LogTrace(message, propertyValue);
		}

		public void LogError(Exception exception, string message)
		{
			_logger?.LogError(exception, message);
		}
		
		public void LogError<T>(Exception exception, string message, T propertyValue)
		{
			_logger?.LogError(message, propertyValue);
		}

		public void LogWarning(string message)
		{
			_logger?.LogWarning(message);
		}

		public void LogWarning<T>(string message, T propertyValue)
		{
			_logger?.LogWarning(message, propertyValue);
		}
	}
}
