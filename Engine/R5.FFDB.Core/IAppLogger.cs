using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core
{
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
