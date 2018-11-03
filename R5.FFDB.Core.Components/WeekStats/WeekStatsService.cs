using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using R5.FFDB.Core.Abstractions;
using R5.FFDB.Core.Components.WeekStats.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace R5.FFDB.Core.Components.WeekStats
{
	public class WeekStatsService
	{
		private const string weekStatsFileName = @"^\d{4}-\d{1,2}.json$";
		private FfdbConfig _config { get; }

		public WeekStatsService(FfdbConfig config)
		{
			_config = config;
		}

		private static string GetApiUri(int season, int week)
		{
			return $"http://api.fantasy.nfl.com/v2/players/weekstats?season={season}&week={week}";
		}

		private static string GetJsonPath(WeekInfo week, string downloadPath)
		{
			if (!downloadPath.EndsWith(@"\"))
			{
				downloadPath += @"\";
			}

			return downloadPath + $"{week.Season}-{week.Week}.json";
		}

		public Core.Stats.WeekStats GetStats(WeekInfo week)
		{
			string path = GetJsonPath(week, _config.WeekStatsDownloadPath);

			var json = JsonConvert.DeserializeObject<WeekStatsJson>(File.ReadAllText(path));

			return WeekStatsJson.ToCoreEntity(json);
		}

		public async Task SaveWeekStatFilesAsync()
		{
			WeekInfo latestCompleted = await GetLatestAvailableWeekAsync();

			List<WeekInfo> missingWeeks = GetMissingWeeks(latestCompleted);

			foreach (WeekInfo week in missingWeeks)
			{
				string uri = GetApiUri(week.Season, week.Week);
				string weekStats = await Http.Client.GetStringAsync(uri);

				saveFile(weekStats, week);

				await Task.Delay(_config.RequestDelayMilliseconds);
			}

			void saveFile(string statsJson, WeekInfo week)
			{
				string path = GetJsonPath(week, _config.WeekStatsDownloadPath);

				if (File.Exists(path))
				{
					throw new InvalidOperationException($"Week stats file already exists for {week.Season} - {week.Week}.");
				}

				File.WriteAllText(path, statsJson);
			}
		}

		private async Task<WeekInfo> GetLatestAvailableWeekAsync()
		{
			// doesn't matter which week we choose, it'll always return
			// the NFL's current state info
			JObject weekStats = await getWeekStatsAsync();

			(int currentSeason, int currentWeek) = getCurrentWeekInfo(weekStats);

			return new WeekInfo(currentSeason, currentWeek);

			// local functions
			async Task<JObject> getWeekStatsAsync()
			{
				string uri = GetApiUri(2018, 1);

				string weekStatsJson = await Http.Client.GetStringAsync(uri);

				return JObject.Parse(weekStatsJson);
			}

			(int season, int week) getCurrentWeekInfo(JObject stats)
			{
				JObject games = stats["games"].ToObject<JObject>();

				string gameId = games.Properties().Select(p => p.Name).First();

				int season = games[gameId]["season"].ToObject<int>();
				int week = games[gameId]["state"]["week"].ToObject<int>();

				bool isCompleted = games[gameId]["state"]["isWeekGamesCompleted"].ToObject<bool>();
				if (!isCompleted)
				{
					week = week - 1;
				}

				return (season, week);
			}
		}

		private List<WeekInfo> GetMissingWeeks(WeekInfo latestAvailable)
		{
			List<WeekInfo> allPossibleWeeks = getAllPossibleWeeks(latestAvailable);
			HashSet<WeekInfo> existingWeeks = getExistingWeeks();

			return allPossibleWeeks.Where(w => !existingWeeks.Contains(w)).ToList();

			// local functions
			List<WeekInfo> getAllPossibleWeeks(WeekInfo latest)
			{
				var result = new List<WeekInfo>();

				// Earliest available is 2010-1
				for (int season = 2010; season < latest.Season; season++)
				{
					for (int week = 1; week <= 17; week++)
					{
						result.Add(new WeekInfo(season, week));
					}
				}

				for (int week = 1; week <= latest.Week; week++)
				{
					result.Add(new WeekInfo(latest.Season, week));
				}

				return result;
			}

			HashSet<WeekInfo> getExistingWeeks()
			{
				var directory = new DirectoryInfo(_config.WeekStatsDownloadPath);
				FileInfo[] files = directory.GetFiles();

				List<string> fileNames = files.Select(f => f.Name).ToList();

				bool namesAreValid = fileNames.All(n => Regex.IsMatch(n, weekStatsFileName));
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
}
