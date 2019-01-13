using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace R5.FFDB.Components
{
	public class DataDirectoryPath
	{
		private string _root { get; }
		public StaticDataDirectoryPath Static { get; }
		public TempDataDirectoryPath Temp { get; }

		public DataDirectoryPath(string rootPath)
		{
			if (string.IsNullOrWhiteSpace(rootPath) || !Directory.Exists(rootPath))
			{
				throw new ArgumentException($"Path '{rootPath}' is invalid.", nameof(rootPath));
			}

			_root = rootPath.EndsWith(@"\") ? rootPath : rootPath + @"\";

			Static = new StaticDataDirectoryPath(_root);
			Temp = new TempDataDirectoryPath(_root);

			CreateMissing();
		}

		private void CreateMissing()
		{
			Static.CreateMissing();
			Temp.CreateMissing();
		}

		public class StaticDataDirectoryPath
		{
			private string _root { get; }
			public string WeekStats => _root + @"week_stats\";
			public string TeamGameHistoryWeekGames => _root + @"team_game_history\week_games\";
			public string TeamGameHistoryGameStats => _root + @"team_game_history\game_stats\";

			public StaticDataDirectoryPath(string rootPath)
			{
				_root = rootPath;
			}

			public void CreateMissing()
			{
				
				Directory.CreateDirectory(WeekStats);
				Directory.CreateDirectory(TeamGameHistoryWeekGames);
				Directory.CreateDirectory(TeamGameHistoryGameStats);
			}
		}

		public class TempDataDirectoryPath
		{
			private string _root { get; }
			public string Player => _root + @"temp\player\";
			public string RosterPages => _root + @"temp\roster_pages\";

			public TempDataDirectoryPath(string rootPath)
			{
				_root = rootPath;
			}

			public void CreateMissing()
			{
				Directory.CreateDirectory(Player);
				Directory.CreateDirectory(RosterPages);
			}
		}
	}
}
