using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core
{
	/// <summary>
	/// Represents the logger instance that's used for the built-in database providers.
	/// This is also provided for use in your own custom providers.
	/// </summary>
	public interface IAppLogger
	{
		void LogInformation(string messageTemplate);
		void LogInformation<T>(string messageTemplate, T propertyValue);
		void LogDebug(string messageTemplate);
		void LogDebug<T>(string messageTemplate, T propertyValue);
		void LogTrace(string messageTemplate);
		void LogTrace<T>(string messageTemplate, T propertyValue);
		void LogError(Exception exception, string messageTemplate);
		void LogError<T>(Exception exception, string messageTemplate, T propertyValue);
		void LogWarning(string messageTemplate);
		void LogWarning<T>(string messageTemplate, T propertyValue);
	}
}
