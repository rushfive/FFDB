using Newtonsoft.Json;
using R5.FFDB.Core.Abstractions;
using R5.FFDB.Core.Components.FantasyApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace R5.FFDB.Core.Components.FantasyApi.Services
{
	public class FileService
	{
		private const string weekStatsFileName = @"^\d{4}-\d{1,2}.json$";

		private FantasyApiConfig _config { get; }

		public FileService(FantasyApiConfig config)
		{
			_config = config;
		}

		private string GetJsonPath(WeekInfo week)
		{
			string path = _config.DownloadPath;
			if (!path.EndsWith(@"\"))
			{
				path += @"\";
			}

			return path + $"{week.Season}-{week.Week}.json";
		}

		public WeekStatsJsonV2 GetWeekStats(WeekInfo week)
		{
			string path = GetJsonPath(week);

			return JsonConvert.DeserializeObject<WeekStatsJsonV2>(File.ReadAllText(path));
		}

		public void SaveWeekStatsToDisk(string statsJson, WeekInfo week)
		{
			string path = GetJsonPath(week);

			if (File.Exists(path))
			{
				throw new InvalidOperationException($"Week stats file already exists for {week.Season} - {week.Week}.");
			}

			File.WriteAllText(path, statsJson);
		}

		// get available weeks that currently dont have its stats saved on disk
		public List<WeekInfo> GetMissingWeeks(WeekInfo latestCompleted)
		{
			List<WeekInfo> allPossibleWeeks = GetAllPossibleWeeks(latestCompleted);
			HashSet<WeekInfo> existingWeeks = GetExistingWeeks();

			return allPossibleWeeks.Where(w => !existingWeeks.Contains(w)).ToList();
		}
		
		private List<WeekInfo> GetAllPossibleWeeks(WeekInfo latestCompleted)
		{
			var result = new List<WeekInfo>();

			// Earliest available is 2010-1
			for (int season = 2010; season < latestCompleted.Season; season++)
			{
				for (int week = 1; week <= 17; week++)
				{
					result.Add(new WeekInfo(season, week));
				}
			}

			for (int week = 1; week <= latestCompleted.Week; week++)
			{
				result.Add(new WeekInfo(latestCompleted.Season, week));
			}

			return result;
		}
		
		private HashSet<WeekInfo> GetExistingWeeks()
		{
			var directory = new DirectoryInfo(_config.DownloadPath);
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

			return fileNames.Select(parseWeekInfo).ToHashSet();
		}
	}
}
