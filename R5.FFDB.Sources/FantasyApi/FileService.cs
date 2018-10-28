using R5.FFDB.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace R5.FFDB.Sources.FantasyApi
{
	public class FileService
	{
		private const string weekStatsFileName = @"^\d{4}-\d{1,2}.json$";
		private readonly WeekInfo earliestWeek = new WeekInfo(2010, 1);

		private FantasyApiSourceConfig _config { get; }

		public FileService(FantasyApiSourceConfig config)
		{
			_config = config;
		}

		// check the downloads dir and find any missing
		public List<WeekInfo> GetExistingWeeks(WeekInfo latestAvailable)
		{
			// temp: hardcode
			latestAvailable = new WeekInfo(2018, 7);
			string path = @"D:\Repos\ffdb_weekstat_downloads\";

			var directory = new DirectoryInfo(path);
			FileInfo[] files = directory.GetFiles();

			List<string> fileNames = files.Select(f => f.Name).ToList();

			bool namesAreValid = fileNames.All(n => Regex.IsMatch(n, FileService.weekStatsFileName));
			if (!namesAreValid)
			{
				throw new InvalidOperationException("There are some invalid week stat files. Remove them from the directory and try again.");
			}

			Func<string, WeekInfo> parseWeekInfo = fileName =>
			{
				string[] dotSplit = fileName.Split(".");
				string[] dashSplit = dotSplit[0].Split("-");

				return new WeekInfo(int.Parse(dashSplit[0]), int.Parse(dashSplit[1]));
			};

			return fileNames.Select(parseWeekInfo).ToList();
		}

	}
}
