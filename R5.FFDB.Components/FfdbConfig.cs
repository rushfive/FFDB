using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components
{
	public class FfdbConfig
	{
		public string WeekStatsDownloadPath { get; set; } = @"D:\Repos\ffdb_weekstat_downloads\"; // temp hardcoded
		public int RequestDelayMilliseconds { get; set; } = 2000; // for safety, dont wanna be banned from the API
		public string PlayerDataPath { get; set; } = @"D:\Repos\ffdb_playerdata\";
	}
}
