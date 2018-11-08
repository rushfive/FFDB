using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Engine
{
	public class FfdbEngine
	{
		// leave as public for now,
		// but should be configured with builder
		public FfdbEngine()
		{

		}

	}

	public class EngineSetup
	{
		private int _requestThrottleMilliseconds { get; set; }

		private string _weekStatsDownloadPath { get; set; }
		private string _playerDataDownloadPath { get; set; }

		



		public FfdbEngine Create()
		{

		}
	}

	public class LoggingConfig
	{
		public string LogDirectory { get; set; }
		public int? WriteInterval { get; set; }
		public long? MaxBytes { get; set; }
	}
}
