using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace R5.FFDB.Components.ErrorFileLog
{
	public interface IErrorFileLogger
	{
		void LogPlayerProfileFetchError(string nflId, Exception exception);
	}

	public class ErrorFileLogger : IErrorFileLogger
	{
		private DataDirectoryPath _dataPath { get; }

		public ErrorFileLogger(DataDirectoryPath dataPath)
		{
			_dataPath = dataPath;
		}

		// Writing
		public void LogPlayerProfileFetchError(string nflId, Exception exception)
		{
			var error = new PlayerProfileFetchError
			{
				NflId = nflId,
				Exception = ErrorFileException.FromException(exception)
			};

			string serializedErrorLog = JsonConvert.SerializeObject(error);

			string path = _dataPath.Error.PlayerProfileFetch + $"{nflId}.json";
			File.WriteAllText(path, serializedErrorLog);
		}

		// Reading
	}

	public enum ErrorType
	{
		PlayerProfile
	}

	public class PlayerProfileFetchError
	{
		[JsonProperty("nflId")]
		public string NflId { get; set; }

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
