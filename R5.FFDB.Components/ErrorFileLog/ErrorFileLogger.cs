using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace R5.FFDB.Components.ErrorFileLog
{
	public interface IErrorFileLogger
	{
		// writing
		void LogPlayerProfileFetchError(string nflId, Exception exception);

		// reading
		List<PlayerProfileFetchError> GetPlayerProfileFetchErrors();
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
				DateTime = DateTime.UtcNow,
				Exception = ErrorFileException.FromException(exception)
			};

			string serializedErrorLog = JsonConvert.SerializeObject(error, Formatting.Indented);

			string path = _dataPath.Error.PlayerProfileFetch + $"{nflId}.json";
			File.WriteAllText(path, serializedErrorLog);
		}

		// Reading
		public List<PlayerProfileFetchError> GetPlayerProfileFetchErrors()
		{
			var result = new List<PlayerProfileFetchError>();

			var directory = new DirectoryInfo(_dataPath.Error.PlayerProfileFetch);
			IEnumerable<string> errorFilePaths = directory.GetFiles().Select(f => f.FullName);

			foreach(string path in errorFilePaths)
			{
				PlayerProfileFetchError error = JsonConvert.DeserializeObject<PlayerProfileFetchError>(File.ReadAllText(path));
				result.Add(error);
			}
			
			return result;
		}
	}

	public enum ErrorType
	{
		PlayerProfile
	}

	public class PlayerProfileFetchError
	{
		[JsonProperty("nflId")]
		public string NflId { get; set; }

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
