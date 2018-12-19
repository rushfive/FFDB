﻿using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.TeamGameHistory.Models;
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
		Task FetchAndSaveAsync();
	}

	public class TeamGameHistorySource : ITeamGameHistorySource
	{
		public string Label => "Team Game History";

		private ILogger<TeamGameHistorySource> _logger { get; }
		private DataDirectoryPath _dataPath { get; }
		private IWebRequestClient _webRequestClient { get; }
		private WebRequestThrottle _throttle { get; }
		private IAvailableWeeksResolver _availableWeeks { get; }
		private GameWeekMap _gameWeekMap { get; }

		public TeamGameHistorySource(
			ILogger<TeamGameHistorySource> logger,
			DataDirectoryPath dataPath,
			IWebRequestClient webRequestClient,
			WebRequestThrottle throttle,
			IAvailableWeeksResolver availableWeeks,
			GameWeekMap gameWeekMap)
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
			await FetchAndSaveWeekGamesAsync();
			await FetchAndSaveGameStatsAsync();
		}

		private async Task FetchAndSaveWeekGamesAsync()
		{
			HashSet<WeekInfo> existing = DirectoryFilesResolver
				.GetWeeksFromXmlFiles(_dataPath.Static.TeamGameHistoryWeekGames)
				.ToHashSet();

			List<WeekInfo> missingWeeks = await _availableWeeks.GetAsync(excludeWeeks: existing);

			foreach (WeekInfo week in missingWeeks)
			{
				string uri = Endpoints.Api.ScoreStripWeekGames(week.Season, week.Week);
				string response = await _webRequestClient.GetStringAsync(uri, throttle: false);

				string fileName = _dataPath.Static.TeamGameHistoryWeekGames + $"{week.Season}-{week.Week}.xml";
				await File.WriteAllTextAsync(fileName, response);

				await _throttle.DelayAsync();
			}
		}

		private async Task FetchAndSaveGameStatsAsync()
		{
			HashSet<string> existing = DirectoryFilesResolver
				.GetFileNames(_dataPath.Static.TeamGameHistoryGameStats, excludeExtensions: true)
				.ToHashSet();

			List<string> gameIds = _gameWeekMap.Get().Keys.ToList();

			foreach (string gameId in gameIds)
			{
				if (existing.Contains(gameId))
				{
					continue;
				}

				string uri = Endpoints.Api.GameCenterStats(gameId);
				string response = await _webRequestClient.GetStringAsync(uri, throttle: false);

				string fileName = _dataPath.Static.TeamGameHistoryGameStats + $"{gameId}.json";
				await File.WriteAllTextAsync(fileName, response);

				existing.Add(gameId);
				await _throttle.DelayAsync();
			}
		}

		public Task CheckHealthAsync()
		{
			// Todo:
			return Task.CompletedTask;
		}
	}
}