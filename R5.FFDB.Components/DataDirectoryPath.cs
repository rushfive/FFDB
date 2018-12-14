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
		public StaticDataDirectoryPath Static { get; }
		public TempDataDirectoryPath Temp { get; }
		public ErrorFileLogDirectoryPath Error { get; }
		//public string PlayerData => _root + @"player_data\";
		//public string WeekStats => _root + @"week_stats\";

		// temp - always okay to dump all
		//public string RosterPages => _root + @"temp\roster_pages\";

		// Error file logs
		//public string 

		public DataDirectoryPath(string rootPath)
		{
			if (string.IsNullOrWhiteSpace(rootPath) || !Directory.Exists(rootPath))
			{
				throw new ArgumentException($"Path '{rootPath}' is invalid.", nameof(rootPath));
			}

			_root = rootPath.EndsWith(@"\") ? rootPath : rootPath + @"\";

			Static = new StaticDataDirectoryPath(_root);
			Temp = new TempDataDirectoryPath(_root);
			Error = new ErrorFileLogDirectoryPath(_root);

			CreateMissing();
		}

		private void CreateMissing()
		{
			Static.CreateMissing();
			Temp.CreateMissing();
			Error.CreateMissing();
		}

		public class StaticDataDirectoryPath
		{
			private string _root { get; }
			public string PlayerProfile => _root + @"player_profile\";
			public string PlayerTeamHistory => _root + @"player_team_history\";
			public string WeekStats => _root + @"week_stats\";
			public string TeamGameHistoryWeekGames => _root + @"team_game_history\week_games\";
			public string TeamGameHistoryGameStats => _root + @"team_game_history\game_stats\";

			public StaticDataDirectoryPath(string rootPath)
			{
				_root = rootPath;
			}

			public void CreateMissing()
			{
				Directory.CreateDirectory(PlayerProfile);
				Directory.CreateDirectory(PlayerTeamHistory);
				Directory.CreateDirectory(WeekStats);
				Directory.CreateDirectory(TeamGameHistoryWeekGames);
				Directory.CreateDirectory(TeamGameHistoryGameStats);
			}
		}

		public class TempDataDirectoryPath
		{
			private string _root { get; }
			public string RosterPages => _root + @"temp\roster_pages\";

			public TempDataDirectoryPath(string rootPath)
			{
				_root = rootPath;
			}

			public void CreateMissing()
			{
				Directory.CreateDirectory(RosterPages);
			}
		}

		public class ErrorFileLogDirectoryPath
		{
			private string _root { get; }
			public string PlayerProfileFetch => _root + @"error_file_logs\player_profile_fetch\";
			public string PlayerTeamHistoryFetch => _root + @"error_file_logs\player_team_history\fetch\";
			public string PlayerTeamHistoryWeekUnavailable => _root + @"error_file_logs\player_team_history\week_unavailable\";

			public ErrorFileLogDirectoryPath(string rootPath)
			{
				_root = rootPath;
			}

			public void CreateMissing()
			{
				Directory.CreateDirectory(PlayerProfileFetch);
				Directory.CreateDirectory(PlayerTeamHistoryFetch);
				Directory.CreateDirectory(PlayerTeamHistoryWeekUnavailable);
			}
		}
	}
}
