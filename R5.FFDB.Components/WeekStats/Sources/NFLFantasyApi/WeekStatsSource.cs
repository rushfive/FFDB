using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.WeekStats.Sources.NFLFantasyApi.Models;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace R5.FFDB.Components.WeekStats.Sources.NFLFantasyApi
{
	public class WeekStatsSource : IWeekStatsSource
	{
		private const string weekStatsFileName = @"^\d{4}-\d{1,2}.json$";

		private ILogger<WeekStatsSource> _logger { get; }
		private DataDirectoryPath _dataPath { get; }
		private IWebRequestClient _webRequestClient { get; }

		public WeekStatsSource(
			ILogger<WeekStatsSource> logger,
			DataDirectoryPath dataPath,
			IWebRequestClient webRequestClient)
		{
			_logger = logger;
			_dataPath = dataPath;
			_webRequestClient = webRequestClient;
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

		public Core.Models.WeekStats GetStats(WeekInfo week)
		{
			string path = GetJsonPath(week, _dataPath.WeekStats);

			var json = JsonConvert.DeserializeObject<WeekStatsJson>(File.ReadAllText(path));

			return WeekStatsJson.ToCoreEntity(json);
		}

		public List<Core.Models.WeekStats> GetAll()
		{
			return GetExistingWeeks()
				.Select(w => GetStats(w))
				.ToList();
		}

		public async Task FetchAndSaveWeekStatsAsync()
		{
			WeekInfo latestCompleted = await GetLatestAvailableWeekAsync();

			_logger.LogInformation($"Fetching all available week stats for players up to and including week {latestCompleted.Week}, {latestCompleted.Season}.");

			List<WeekInfo> missingWeeks = GetMissingWeeks(latestCompleted);

			IEnumerable<string> missing = missingWeeks.Select(w => $"{w.Season}-{w.Week}");
			_logger.LogDebug($"Fetching for {missingWeeks.Count} weeks that are missing: {string.Join(", ", missing)}");

			foreach (WeekInfo week in missingWeeks)
			{
				_logger.LogDebug($"Beginning week stats fetch for {week.Season}-{week.Week}.");

				string uri = GetApiUri(week.Season, week.Week);

				string weekStats = null;
				try
				{
					weekStats = await _webRequestClient.GetStringAsync(uri);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, $"Failed to fetch week stats from '{uri}'.");
					throw;
				}

				try
				{
					saveFile(weekStats, week);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to save week stats to disk.", weekStats);
					throw;
				}

				_logger.LogInformation($"Successfully saved week stats for {week.Season}-{week.Week}.");
			}

			_logger.LogInformation("Successfully fetched all available week stats.");

			// local functions
			void saveFile(string statsJson, WeekInfo week)
			{
				string path = GetJsonPath(week, _dataPath.WeekStats);

				if (File.Exists(path))
				{
					throw new InvalidOperationException($"Week stats file already exists for {week.Season}-{week.Week} at path '{path}'.");
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

				string weekStatsJson = await _webRequestClient.GetStringAsync(uri, throttle: false);

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
			HashSet<WeekInfo> existingWeeks = GetExistingWeeks().ToHashSet();

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
		}

		private IEnumerable<WeekInfo> GetExistingWeeks()
		{
			var directory = new DirectoryInfo(_dataPath.WeekStats);
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

			return fileNames.Select(parseWeekInfo);
		}

		public Task<bool> IsHealthyAsync()
		{
			// todo:
			return Task.FromResult(true);
		}
	}
}
