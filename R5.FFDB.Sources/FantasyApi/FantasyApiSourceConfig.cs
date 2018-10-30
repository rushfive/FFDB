using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Sources.FantasyApi
{
	public class FantasyApiSourceConfig
	{
		public string DownloadPath { get; set; } = @"D:\Repos\ffdb_weekstat_downloads\"; // temp hardcoded
		public int RequestDelayMilliseconds { get; set; } = 1000; // for safety, dont wanna be banned from the API
	}
}
