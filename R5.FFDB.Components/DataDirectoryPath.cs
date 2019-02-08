using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace R5.FFDB.Components
{
	public class DataDirectoryPath
	{
		private string _root { get; }
		public StaticPaths Static { get; } // REMOVE LATER
		public TempPaths Temp { get; } // REMOVE LATER

		private string _weekGameMap { get; }
		private string _players { get; }

		public DataDirectoryPath(string rootPath)
		{
			if (string.IsNullOrWhiteSpace(rootPath) || !Directory.Exists(rootPath))
			{
				throw new ArgumentException($"Path '{rootPath}' is invalid.", nameof(rootPath));
			}

			_root = rootPath.EndsWith(@"\") ? rootPath : rootPath + @"\";

			Static = new StaticPaths(_root);
			Temp = new TempPaths(_root);

			//CreateMissing();

			// OLD ABOVE

			_weekGameMap = _root + @"week_game_map\";
			_players = _root + @"players\";
			CreateMissingPaths();
		}

		public string WeekGameMap(WeekInfo week)
		{
			return _weekGameMap + $"{week.Season}-{week.Week}.json";
		}

		public string Player(string nflId)
		{
			return _players + $"{nflId}.json";
		}

		private void CreateMissingPaths()
		{
			Directory.CreateDirectory(_weekGameMap);
			Directory.CreateDirectory(_players);
		}




		// OLD BELOW, WILL ALL BE REPLACED EVENTUALLY

		private void CreateMissing()
		{
			Static.CreateMissing();
			Temp.CreateMissing();
		}

		public class StaticPaths
		{
			private string _root { get; }
			public string WeekStats => _root + @"week_stats\";

			// TODO: REMOVE THIS
			public string WeekGames => _root + @"week_games\";


			public string TeamGameStats => _root + @"team_game_stats\";
			public string WeekGameMatchups => _root + @"week_game_matchups\";

			public StaticPaths(string rootPath)
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

		public class TempPaths
		{
			private string _root { get; }
			public string Player => _root + @"player\";
			public string RosterPages => _root + @"roster_pages\";

			public TempPaths(string rootPath)
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
