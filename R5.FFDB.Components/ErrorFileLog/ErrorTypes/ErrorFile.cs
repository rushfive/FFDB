using Newtonsoft.Json;
using System;

namespace R5.FFDB.Components.ErrorFileLog.ErrorTypes
{
	public abstract class ErrorFile
	{
		[JsonProperty("dateTime")]
		public DateTime DateTime { get; set; }

		[JsonProperty("exception")]
		public ErrorFileException Exception { get; set; }
	}

	public class ErrorFileException
	{
		[JsonProperty("message")]
		public string Message { get; set; }

		[JsonProperty("stackTrace")]
		public string StackTrace { get; set; }

		public static ErrorFileException FromException(Exception ex)
		{
			return new ErrorFileException
			{
				Message = ex.Message,
				StackTrace = ex.StackTrace
			};
		}
	}
}
