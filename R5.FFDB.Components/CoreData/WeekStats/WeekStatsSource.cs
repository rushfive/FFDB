using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R5.FFDB.Components.CoreData.WeekStats.Models;
using R5.FFDB.Components.Resolvers;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.WeekStats
{
	public interface IWeekStatsSource : ISource
	{
		Core.Models.WeekStats GetStats(WeekInfo week);
		List<Core.Models.WeekStats> GetAll();
		Task FetchAndSaveWeekStatsAsync();
	}

	public class WeekStatsSource : IWeekStatsSource
	{
		private ILogger<WeekStatsSource> _logger { get; }
		private DataDirectoryPath _dataPath { get; }
		private IWebRequestClient _webRequestClient { get; }
		private IAvailableWeeksResolver _availableWeeks { get; }
		private LatestWeekValue _latestWeek { get; }

		public WeekStatsSource(
			ILogger<WeekStatsSource> logger,
			DataDirectoryPath dataPath,
			IWebRequestClient webRequestClient,
			IAvailableWeeksResolver availableWeeks,
			LatestWeekValue latestWeek)
		{
			_logger = logger;
			_dataPath = dataPath;
			_webRequestClient = webRequestClient;
			_availableWeeks = availableWeeks;
			_latestWeek = latestWeek;
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
			string path = GetJsonPath(week, _dataPath.Static.WeekStats);

			var json = JsonConvert.DeserializeObject<WeekStatsJson>(File.ReadAllText(path));

			return WeekStatsJson.ToCoreEntity(json);
		}

		public List<Core.Models.WeekStats> GetAll()
		{
			return DirectoryFilesResolver
				.GetWeeksFromJsonFiles(_dataPath.Static.WeekStats)
				.Select(w => GetStats(w))
				.ToList();
		}

		public async Task FetchAndSaveWeekStatsAsync()
		{
			WeekInfo latestCompleted = await _latestWeek.GetAsync();

			_logger.LogInformation($"Fetching all available week stats for players up to and including week {latestCompleted.Week}, {latestCompleted.Season}.");

			HashSet<WeekInfo> existingWeeks = DirectoryFilesResolver
				.GetWeeksFromJsonFiles(_dataPath.Static.WeekStats)
				.ToHashSet();

			List<WeekInfo> missingWeeks = await _availableWeeks.GetAsync(excludeWeeks: existingWeeks);

			if (!missingWeeks.Any())
			{
				_logger.LogInformation("Already have all available week stats - no fetching necessary.");
				return;
			}

			IEnumerable<string> missing = missingWeeks.Select(w => $"{w.Season}-{w.Week}");
			_logger.LogDebug($"Fetching for {missingWeeks.Count} weeks that are missing: {string.Join(", ", missing)}");

			foreach (WeekInfo week in missingWeeks)
			{
				_logger.LogDebug($"Beginning week stats fetch for {week.Season}-{week.Week}.");

				string uri = Endpoints.Api.WeekStats(week.Season, week.Week);

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
				string path = GetJsonPath(week, _dataPath.Static.WeekStats);

				if (File.Exists(path))
				{
					throw new InvalidOperationException($"Week stats file already exists for {week.Season}-{week.Week} at path '{path}'.");
				}

				File.WriteAllText(path, statsJson);
			}
		}

		public Task<bool> IsHealthyAsync()
		{
			// todo:
			return Task.FromResult(true);
		}
	}
}
