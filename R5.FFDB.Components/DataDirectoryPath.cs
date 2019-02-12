using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace R5.FFDB.Components
{
	public class DataDirectoryPath
	{
		public SourceFilePaths SourceFiles { get; }
		public VersionedFilePaths Versioned { get; }

		public DataDirectoryPath(string rootPath)
		{
			if (string.IsNullOrWhiteSpace(rootPath) || !Directory.Exists(rootPath))
			{
				throw new ArgumentException($"Path '{rootPath}' is invalid.", nameof(rootPath));
			}

			rootPath = rootPath.EndsWith("\\") ? rootPath : rootPath + "\\";

			SourceFiles = new SourceFilePaths(rootPath);
			Versioned = new VersionedFilePaths(rootPath);
		}
	}

	public class SourceFilePaths
	{
		public Paths V1 { get; }

		public SourceFilePaths(string rootPath)
		{
			var versionedRoot = rootPath + "original_source\\";

			V1 = new Paths(versionedRoot, 1);
		}

		public class Paths
		{
			private string _weekMatchup { get; }
			private string _playerAdd { get; }
			private string _playerWeekStats { get; }
			private string _teamStats { get; }

			public Paths(string versionedRootPath, int version)
			{
				var root = versionedRootPath + $"V{version}\\";

				_weekMatchup = versionedRootPath + "week_matchup\\";
				_playerAdd = versionedRootPath + "player_add\\";
				_teamStats = versionedRootPath + "team_stats\\";
				_playerWeekStats = versionedRootPath + "player_week_stats\\";

				CreateMissingPaths();
			}

			public string WeekMatchup(WeekInfo week)
			{
				return _weekMatchup + $"{week.Season}-{week.Week}.json";
			}

			public string PlayerAdd(string nflId)
			{
				return _playerAdd + $"{nflId}.json";
			}

			public string PlayerWeekStats(WeekInfo week)
			{
				return _playerWeekStats + $"{week}.json";
			}

			public string TeamStats(string gameId)
			{
				return _teamStats + $"{gameId}.json";
			}

			private void CreateMissingPaths()
			{
				Directory.CreateDirectory(_playerWeekStats);
			}
		}
	}

	public class VersionedFilePaths
	{
		public Paths V1 { get; }

		public VersionedFilePaths(string rootPath)
		{
			var versionedRoot = rootPath + "versioned\\";

			V1 = new Paths(versionedRoot, 1);
		}

		public class Paths
		{
			private string _weekMatchup { get; }
			private string _playerAdd { get; }
			private string _rosters { get; }
			private string _teamStats { get; }
			private string _playerWeekStats { get; }

			public Paths(string versionedRootPath, int version)
			{
				var root = versionedRootPath + $"V{version}\\";

				_weekMatchup = versionedRootPath + "week_matchup\\";
				_playerAdd = versionedRootPath + "player_add\\";
				_rosters = versionedRootPath + "rosters\\";
				_teamStats = versionedRootPath + "team_stats\\";
				_playerWeekStats = versionedRootPath + "player_week_stats\\";

				CreateMissingPaths();
			}

			public string WeekMatchup(WeekInfo week)
			{
				return _weekMatchup + $"{week.Season}-{week.Week}.json";
			}

			public string PlayerAdd(string nflId)
			{
				return _playerAdd + $"{nflId}.json";
			}

			public string Roster(Team team)
			{
				return _rosters + $"{team.Abbreviation}.json";
			}

			public string TeamStats(string gameId)
			{
				return _teamStats + $"{gameId}.json";
			}

			public string PlayerWeekStats(WeekInfo week)
			{
				return _playerWeekStats + $"{week}.json";
			}

			private void CreateMissingPaths()
			{
				Directory.CreateDirectory(_weekMatchup);
				Directory.CreateDirectory(_playerAdd);
				Directory.CreateDirectory(_rosters);
				Directory.CreateDirectory(_teamStats);
				Directory.CreateDirectory(_playerWeekStats);
			}
		}
	}
}
