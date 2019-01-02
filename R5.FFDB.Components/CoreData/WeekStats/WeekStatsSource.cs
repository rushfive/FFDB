using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R5.FFDB.Components.CoreData.TeamGameHistory;
using R5.FFDB.Components.CoreData.WeekStats.Models;
using R5.FFDB.Components.Http;
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
	public interface IWeekStatsSource : ICoreDataSource
	{
	}

	public class WeekStatsSource : IWeekStatsSource
	{
		public string Label => "Week Stats";

		private ILogger<WeekStatsSource> _logger { get; }
		private DataDirectoryPath _dataPath { get; }
		private IWebRequestClient _webRequestClient { get; }
		private IAvailableWeeksResolver _availableWeeks { get; }
		private LatestWeekValue _latestWeek { get; }
		private IPlayerWeekTeamMap _playerWeekTeamHistory { get; }

		public WeekStatsSource(
			ILogger<WeekStatsSource> logger,
			DataDirectoryPath dataPath,
			IWebRequestClient webRequestClient,
			IAvailableWeeksResolver availableWeeks,
			LatestWeekValue latestWeek,
			IPlayerWeekTeamMap playerWeekTeamHistory)
		{
			_logger = logger;
			_dataPath = dataPath;
			_webRequestClient = webRequestClient;
			_availableWeeks = availableWeeks;
			_latestWeek = latestWeek;
			_playerWeekTeamHistory = playerWeekTeamHistory;
		}

		public async Task FetchAndSaveAsync()
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
				string path = _dataPath.Static.WeekStats + $"{week.Season}-{week.Week}.json";

				if (File.Exists(path))
				{
					throw new InvalidOperationException($"Week stats file already exists for {week.Season}-{week.Week} at path '{path}'.");
				}

				File.WriteAllText(path, statsJson);
			}
		}

		public Task CheckHealthAsync()
		{
			// Todo:
			return Task.CompletedTask;
		}
	}
}
