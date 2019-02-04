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

			// TODO: REMOVE THIS
			public string WeekGames => _root + @"week_games\";


			public string TeamGameStats => _root + @"team_game_stats\";
			public string WeekGameMatchups => _root + @"week_game_matchups\";

			public StaticDataDirectoryPath(string rootPath)
			{
				_root = rootPath + @"static\";
			}

			public void CreateMissing()
			{
				Directory.CreateDirectory(WeekStats);
				Directory.CreateDirectory(WeekGames);
				Directory.CreateDirectory(TeamGameStats);
				Directory.CreateDirectory(WeekGameMatchups);
			}
		}

		public class TempDataDirectoryPath
		{
			private string _root { get; }
			public string Player => _root + @"player\";
			public string RosterPages => _root + @"roster_pages\";

			public TempDataDirectoryPath(string rootPath)
			{
				_root = rootPath + @"temp\";
			}

			public void CreateMissing()
			{
				Directory.CreateDirectory(Player);
				Directory.CreateDirectory(RosterPages);
			}
		}
	}
}
