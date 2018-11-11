using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using R5.FFDB.Components;
using R5.FFDB.Components.Configurations;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

	public class EngineSetup
	{
		public WebRequestConfigBuilder WebRequest { get; } = new WebRequestConfigBuilder();
		public FileDownloadConfigBuilder FileDownload { get; } = new FileDownloadConfigBuilder();
		public LoggingConfigBuilder Logging { get; } = new LoggingConfigBuilder();



		public FfdbEngine Create()
		{
			WebRequestConfig webRequestConfig = WebRequest.Build();
			FileDownloadConfig fileDownloadConfig = FileDownload.Build();
			LoggingConfig loggingConfig = Logging.Build();

			var services = new ServiceCollection();

			services
				.AddScoped(sp => webRequestConfig)
				.AddScoped(sp => fileDownloadConfig)
				.AddScoped<IWebRequestClient, WebRequestClient>()
				.AddLogging(loggingConfig)
				.AddScoped<FfdbEngine>();

			return services
				.BuildServiceProvider()
				.GetService<FfdbEngine>();
		}
	}

	public static class EngineSetupExtensions
	{
		public static IServiceCollection AddLogging(this IServiceCollection services, LoggingConfig config)
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.WriteTo.Console()
				.WriteTo.File(
					config.LogDirectory + ".txt",
					fileSizeLimitBytes: config.MaxBytes,
					restrictedToMinimumLevel: config.MinimumLogLevel,
					rollingInterval: config.RollingInterval,
					rollOnFileSizeLimit: config.RollOnFileSizeLimit)
				.CreateLogger();

			return services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
		}
	}

	public class WebRequestConfigBuilder
	{
		private int _throttleMilliseconds { get; set; } = 3000;
		private (int min, int max)? _randomizedThrottle { get; set; }
		private Dictionary<string, string> _headers { get; } = new Dictionary<string, string>();

		public WebRequestConfigBuilder SetThrottle(int milliseconds)
		{
			if (milliseconds < 0)
			{
				throw new ArgumentException("Throttle value must be a non-negative value.");
			}

			_throttleMilliseconds = milliseconds;
			return this;
		}

		public WebRequestConfigBuilder SetRandomizedThrottle(int min, int max)
		{
			if (min < 0 || max < 0)
			{
				throw new ArgumentException("Throttle values must be non-negative values.");
			}
			if (max <= min)
			{
				throw new ArgumentException("Max value must be greater than min.");
			}

			_randomizedThrottle = (min, max);
			return this;
		}

		public WebRequestConfigBuilder AddHeader(string key, string value)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				throw new ArgumentNullException(nameof(key), "Header key must be provided.");
			}
			// todo: validate value? or empty ok
			_headers[key] = value;
			return this;
		}

		public WebRequestConfigBuilder AddDefaultBrowserHeaders()
		{
			// todo:
			// https://html-agility-pack.net/knowledge-base/14005175/html-agility-pack--web-scraping--and-spoofing-in-csharp

			var headers = new List<(string, string)>
			{
				( "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8" ),
				( "Accept-Encoding", "gzip, deflate" ),
				( "Accept-Language", "en-US,en;q=0.9" ),
				( "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36" )
			};

			foreach (var (key, value) in headers)
			{
				AddHeader(key, value);
			}

			return this;
		}

		internal WebRequestConfig Build()
		{
			Validate();

			return new WebRequestConfig(
				_throttleMilliseconds,
				_randomizedThrottle,
				_headers);
		}

		private void Validate()
		{
			if (!_randomizedThrottle.HasValue && _throttleMilliseconds < 0)
			{
				throw new InvalidOperationException("Failed to build web request config because "
					+ "the throttle value is invalid.");
			}
		}
	}

	public class FileDownloadConfigBuilder
	{
		private string _weekStatsDirectory { get; set; }
		private string _playerDataDirectory { get; set; }

		public FileDownloadConfigBuilder SetWeekStatsDirectory(string directoryPath)
		{
			if (string.IsNullOrWhiteSpace(directoryPath))
			{
				throw new ArgumentNullException(nameof(directoryPath), "Week Stats directory path must be provided.");
			}
			if (!directoryPath.EndsWith("\\"))
			{
				directoryPath += "\\";
			}
			if (!Directory.Exists(directoryPath))
			{
				throw new ArgumentException($"Directory path '{directoryPath}' doesn't exist.");
			}

			_weekStatsDirectory = directoryPath;
			return this;
		}

		public FileDownloadConfigBuilder SetPlayerDataDirectory(string directoryPath)
		{
			if (string.IsNullOrWhiteSpace(directoryPath))
			{
				throw new ArgumentNullException(nameof(directoryPath), "Player Data directory path must be provided.");
			}
			if (!directoryPath.EndsWith("\\"))
			{
				directoryPath += "\\";
			}
			if (!Directory.Exists(directoryPath))
			{
				throw new ArgumentException($"Directory path '{directoryPath}' doesn't exist.");
			}

			_playerDataDirectory = directoryPath;
			return this;
		}

		internal FileDownloadConfig Build()
		{
			Validate();

			return new FileDownloadConfig
			{
				PlayerData = _playerDataDirectory,
				WeekStats = _weekStatsDirectory
			};
		}

		private void Validate()
		{
			if (string.IsNullOrWhiteSpace(_weekStatsDirectory))
			{
				throw new InvalidOperationException("Week Stats directory path must be set.");
			}
			if (string.IsNullOrWhiteSpace(_playerDataDirectory))
			{
				throw new InvalidOperationException("Player Data directory path must be set.");
			}
			if (_weekStatsDirectory == _playerDataDirectory)
			{
				throw new InvalidOperationException("Directory paths for Week Stats and Player Data must be different.");
			}
		}
	}

	public class LoggingConfigBuilder
	{
		private string _logDirectory { get; set; }
		private long? _maxBytes { get; set; }
		private RollingInterval _rollingInterval { get; set; } = RollingInterval.Hour;
		private bool _rollOnFileSizeLimit { get; set; }
		private LogEventLevel _minimumLogLevel { get; set; } = LogEventLevel.Debug;

		public LoggingConfigBuilder SetLogDirectory(string directoryPath)
		{
			if (string.IsNullOrWhiteSpace(directoryPath))
			{
				throw new ArgumentNullException(nameof(directoryPath), "Logging directory path must be provided.");
			}
			if (!directoryPath.EndsWith("\\"))
			{
				directoryPath += "\\";
			}
			if (!Directory.Exists(directoryPath))
			{
				throw new ArgumentException($"Directory path '{directoryPath}' doesn't exist.");
			}

			_logDirectory = directoryPath;
			return this;
		}

		public LoggingConfigBuilder SetMaxBytes(long maxBytes)
		{
			if (maxBytes <= 0)
			{
				throw new ArgumentException("Max bytes value must be greater than 0.");
			}

			_maxBytes = maxBytes;
			return this;
		}

		public LoggingConfigBuilder SetRollingInterval(RollingInterval interval)
		{
			_rollingInterval = interval;
			return this;
		}

		public LoggingConfigBuilder RollOnFileSizeLimit()
		{
			_rollOnFileSizeLimit = true;
			return this;
		}

		public LoggingConfigBuilder SetMinimumLogLevel(LogEventLevel level)
		{
			_minimumLogLevel = level;
			return this;
		}

		internal LoggingConfig Build()
		{
			Validate();

			return new LoggingConfig
			{
				LogDirectory = _logDirectory,
				MaxBytes = _maxBytes,
				RollingInterval = _rollingInterval,
				RollOnFileSizeLimit = _rollOnFileSizeLimit,
				MinimumLogLevel = _minimumLogLevel
			};
		}

		private void Validate()
		{
			if (string.IsNullOrWhiteSpace(_logDirectory))
			{
				throw new InvalidOperationException("Logging directory must be provided.");
			}
		}
	}
}
