using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Configurations;

namespace R5.FFDB.Engine
{
	public class FfdbEngine
	{
		private ILogger<FfdbEngine> _logger { get; }
		private WebRequestConfig _webRequestConfig { get; }
		private FileDownloadConfig _fileDownloadConfig { get; }

		// leave as public for now,
		// but should be configured with builder
		public FfdbEngine(
			ILogger<FfdbEngine> logger,
			WebRequestConfig webRequestConfig,
			FileDownloadConfig fileDownloadConfig)
		{
			_logger = logger;
			_webRequestConfig = webRequestConfig;
			_fileDownloadConfig = fileDownloadConfig;
		}

		public void TestLogging()
		{
			_logger.LogTrace("this is a trace log.");
			_logger.LogDebug("this is a debug log");
			_logger.LogCritical("this is a criterial log");
		}
	}
}
