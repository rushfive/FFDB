using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using R5.FFDB.Components.CoreData.TeamData.Models;
using R5.FFDB.Components.CoreData.TeamGameHistory.Values;
using R5.FFDB.Components.Http;
using R5.FFDB.Components.Resolvers;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace R5.FFDB.Components.CoreData.TeamGameHistory
{
	public interface ITeamGameHistorySource : ICoreDataSource
	{
	}

	public class TeamGameHistorySource : ITeamGameHistorySource
	{
		public string Label => "Team Game History";

		private ILogger<TeamGameHistorySource> _logger { get; }
		private DataDirectoryPath _dataPath { get; }
		private IWebRequestClient _webRequestClient { get; }
		private WebRequestThrottle _throttle { get; }
		private IAvailableWeeksResolver _availableWeeks { get; }
		private GameWeekMapValue _gameWeekMap { get; }

		public TeamGameHistorySource(
			ILogger<TeamGameHistorySource> logger,
			DataDirectoryPath dataPath,
			IWebRequestClient webRequestClient,
			WebRequestThrottle throttle,
			IAvailableWeeksResolver availableWeeks,
			GameWeekMapValue gameWeekMap)
		{
			_logger = logger;
			_dataPath = dataPath;
			_webRequestClient = webRequestClient;
			_throttle = throttle;
			_availableWeeks = availableWeeks;
			_gameWeekMap = gameWeekMap;
		}

		public async Task FetchAndSaveAsync()
		{
			_logger.LogInformation("Beginning fetching of team game history data.");

			await FetchAndSaveWeekGamesAsync();
			await FetchAndSaveGameStatsAsync();

			_logger.LogInformation("Successfully finished fetching of team game history data.");
		}

		private async Task FetchAndSaveWeekGamesAsync()
		{
			_logger.LogDebug("Beginning fetching of weekly games information..");

			HashSet<WeekInfo> existing = DirectoryFilesResolver
				.GetWeeksFromXmlFiles(_dataPath.Static.TeamGameHistoryWeekGames)
				.ToHashSet();

			List<WeekInfo> missingWeeks = await _availableWeeks.GetAsync(excludeWeeks: existing);

			if (!missingWeeks.Any())
			{
				_logger.LogInformation("Already have all available weekly game information.");
				return;
			}

			_logger.LogInformation($"Already have weekly game information for {existing.Count} weeks. "
				+ $"Will begin fetching of remaining {missingWeeks.Count}.");

			foreach (WeekInfo week in missingWeeks)
			{
				string uri = Endpoints.Api.ScoreStripWeekGames(week.Season, week.Week);

				_logger.LogTrace($"Starting request for {week} at endpoint '{uri}'.");
				string response = await _webRequestClient.GetStringAsync(uri, throttle: false);

				string filePath = _dataPath.Static.TeamGameHistoryWeekGames + $"{week.Season}-{week.Week}.xml";

				_logger.LogTrace($"Saving XML response to '{filePath}'.");
				await File.WriteAllTextAsync(filePath, response);

				await _throttle.DelayAsync();

				_logger.LogDebug($"Finished fetching weekly game information for {week}.");
			}

			_logger.LogInformation($"Finished fetching weekly game information for all {missingWeeks.Count} missing weeks.");
		}

		private async Task FetchAndSaveGameStatsAsync()
		{
			_logger.LogDebug("Beginning fetching of weekly game stats information..");

			HashSet<string> existing = DirectoryFilesResolver
				.GetFileNames(_dataPath.Static.TeamGameHistoryGameStats, excludeExtensions: true)
				.ToHashSet();

			List<string> missing = _gameWeekMap.Get().Keys
				.Where(id => !existing.Contains(id))
				.ToList();

			if (!missing.Any())
			{
				_logger.LogInformation("Already have all available stats information.");
				return;
			}

			_logger.LogInformation($"Already have game stats information for {existing.Count} weeks. "
				+ $"Will begin fetching of remaining {missing.Count}.");

			foreach (string gameId in missing)
			{
				if (existing.Contains(gameId))
				{
					continue;
				}

				string uri = Endpoints.Api.GameCenterStats(gameId);

				_logger.LogTrace($"Starting request for game {gameId} at endpoint '{uri}'.");
				string response = await _webRequestClient.GetStringAsync(uri, throttle: false);

				string filePath = _dataPath.Static.TeamGameHistoryGameStats + $"{gameId}.json";

				_logger.LogTrace($"Saving JSON response to '{filePath}'.");
				await File.WriteAllTextAsync(filePath, response);

				existing.Add(gameId);
				await _throttle.DelayAsync();

				_logger.LogDebug($"Finished fetching stats information for game {gameId}.");
			}

			_logger.LogDebug("Finished fetching of weekly game stats information.");
		}

		public Task CheckHealthAsync()
		{
			// Todo:
			return Task.CompletedTask;
		}
	}
}
