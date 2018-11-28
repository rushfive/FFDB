using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace R5.FFDB.Components
{
	public class DataDirectoryPath
	{
		private string _root { get; }

		// static data
		public string PlayerData => _root + @"player_data\";
		public string WeekStats => _root + @"week_stats\";

		// temp - always okay to dump all
		public string RosterPages => _root + @"temp\roster_pages\";

		public DataDirectoryPath(string rootPath)
		{
			if (string.IsNullOrWhiteSpace(rootPath) || !Directory.Exists(rootPath))
			{
				throw new ArgumentException($"Path '{rootPath}' is invalid.", nameof(rootPath));
			}

			_root = rootPath.EndsWith(@"\") ? rootPath : rootPath + @"\";

			CreateMissing();
		}

		private void CreateMissing()
		{
			Directory.CreateDirectory(PlayerData);
			Directory.CreateDirectory(WeekStats);
			Directory.CreateDirectory(RosterPages);
		}
	}
}
