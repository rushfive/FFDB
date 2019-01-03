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
		Task FetchAllAsync();
		Task FetchForWeeksAsync(List<WeekInfo> weeks);
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

		public async Task FetchAllAsync()
		{
			_logger.LogInformation("Beginning fetching of team game history data for all available weeks.");

			List<WeekInfo> availableWeeks = await _availableWeeks.GetAsync();
			
			await FetchForWeeksAsync(availableWeeks);

			_logger.LogInformation("Finished fetching of team game history data for all available weeks.");
		}

		public async Task FetchForWeeksAsync(List<WeekInfo> weeks)
		{
			_logger.LogInformation($"Beginning fetching of team game history data for {weeks.Count} week(s).");
			_logger.LogTrace($"Fetching for weeks: {string.Join(", ", weeks)}");

			foreach(WeekInfo week in weeks)
			{
				await FetchGamesForWeekAsync(week);
				await FetchSaveGameStatsAsync(week);
			}

			_logger.LogInformation("Finished fetching team game history data.");
		}

		private async Task FetchGamesForWeekAsync(WeekInfo week)
		{
			string filePath = _dataPath.Static.TeamGameHistoryWeekGames + $"{week.Season}-{week.Week}.xml";

			if (File.Exists(filePath))
			{
				_logger.LogInformation($"Week games file already exists for {week}. Will not fetch.");
				return;
			}

			string uri = Endpoints.Api.ScoreStripWeekGames(week.Season, week.Week);

			_logger.LogDebug($"Fetching week games for {week} from '{uri}'.");

			string response = await _webRequestClient.GetStringAsync(uri, throttle: false);
			
			_logger.LogTrace($"Saving XML response to '{filePath}'.");
			await File.WriteAllTextAsync(filePath, response);

			_logger.LogInformation($"Finished fetching week games for {week}.");
		}

		private async Task FetchSaveGameStatsAsync(WeekInfo week)
		{
			List<string> gameIds = GetGameIdsForWeek(week);

			_logger.LogDebug($"Fetching game stats for {week}.");
			_logger.LogTrace($"Game ids: {string.Join(", ", gameIds)}");

			foreach (string gameId in gameIds)
			{
				string filePath = _dataPath.Static.TeamGameHistoryGameStats + $"{gameId}.json";

				if (File.Exists(filePath))
				{
					_logger.LogInformation($"Game stats file already exists for game '{gameId}'. Will not fetch.");
					continue;
				}

				string uri = Endpoints.Api.GameCenterStats(gameId);

				_logger.LogTrace($"Starting request for game {gameId} at endpoint '{uri}'.");
				string response = await _webRequestClient.GetStringAsync(uri, throttle: false);
				
				_logger.LogTrace($"Saving JSON response to '{filePath}'.");
				await File.WriteAllTextAsync(filePath, response);
				
				await _throttle.DelayAsync();

				_logger.LogDebug($"Finished fetching game stats data for game '{gameId}'.");
			}

			_logger.LogDebug($"Finished fetching of all game stats data for {week}.");
		}

		private List<string> GetGameIdsForWeek(WeekInfo week)
		{
			var result = new List<string>();

			var filePath = _dataPath.Static.TeamGameHistoryWeekGames + $"{week.Season}-{week.Week}.xml";

			XElement weekGameXml = XElement.Load(filePath);

			XElement gameNode = weekGameXml.Elements("gms").Single();

			foreach (XElement game in gameNode.Elements("g"))
			{
				string gameId = game.Attribute("eid").Value;
				result.Add(gameId);
			}

			return result;
		}

		//

		//public async Task FetchAndSaveForWeekAsync(WeekInfo week)
		//{
		//	string uri = Endpoints.Api.ScoreStripWeekGames(week.Season, week.Week);

		//	_logger.LogInformation($"Beginning fetching of team game history data for {week} from '{uri}'.");

		//	await FetchSaveWeekGamesForWeekAsync(week, uri);

		//	List<string> gameIds = GetGameIdsForWeek(week);
		//	await FetchAndSaveGameStatsAsync(gameIds);

		//	_logger.LogInformation($"Successfully finished fetching team game history data for {week}.");
		//}

		//private List<string> GetGameIdsForWeek(WeekInfo week)
		//{
		//	var result = new List<string>();

		//	var filePath = _dataPath.Static.TeamGameHistoryWeekGames + $"{week.Season}-{week.Week}.xml";

		//	XElement weekGameXml = XElement.Load(filePath);

		//	XElement gameNode = weekGameXml.Elements("gms").Single();

		//	foreach (XElement game in gameNode.Elements("g"))
		//	{
		//		string gameId = game.Attribute("eid").Value;
		//		result.Add(gameId);
		//	}

		//	return result;
		//}

		//public async Task FetchAndSaveAsync()
		//{
		//	_logger.LogInformation("Beginning fetching of team game history data.");

		//	await FetchAndSaveWeekGamesAsync();

		//	List<string> gameIds = _gameWeekMap.Get().Keys.ToList();
		//	await FetchAndSaveGameStatsAsync(gameIds);

		//	_logger.LogInformation("Successfully finished fetching of team game history data.");
		//}

		//private async Task FetchAndSaveWeekGamesAsync()
		//{
		//	_logger.LogDebug("Beginning fetching of weekly games information..");

		//	HashSet<WeekInfo> existing = DirectoryFilesResolver
		//		.GetWeeksFromXmlFiles(_dataPath.Static.TeamGameHistoryWeekGames)
		//		.ToHashSet();

		//	List<WeekInfo> missingWeeks = await _availableWeeks.GetAsync(excludeWeeks: existing);

		//	if (!missingWeeks.Any())
		//	{
		//		_logger.LogInformation("Already have all available weekly game information.");
		//		return;
		//	}

		//	_logger.LogInformation($"Already have weekly game information for {existing.Count} weeks. "
		//		+ $"Will begin fetching of remaining {missingWeeks.Count}.");

		//	foreach (WeekInfo week in missingWeeks)
		//	{
		//		string uri = Endpoints.Api.ScoreStripWeekGames(week.Season, week.Week);

		//		_logger.LogTrace($"Starting request for {week} at endpoint '{uri}'.");

		//		await FetchSaveWeekGamesForWeekAsync(week, uri);
		//		await _throttle.DelayAsync();

		//		_logger.LogDebug($"Finished fetching weekly game information for {week}.");
		//	}

		//	_logger.LogInformation($"Finished fetching weekly game information for all {missingWeeks.Count} missing weeks.");
		//}



		//private async Task FetchAndSaveGameStatsAsync(List<string> gameIds)
		//{
		//	_logger.LogDebug("Beginning fetching of weekly game stats data..");

		//	HashSet<string> existing = DirectoryFilesResolver
		//		.GetFileNames(_dataPath.Static.TeamGameHistoryGameStats, excludeExtensions: true)
		//		.ToHashSet();

		//	List<string> missing = gameIds
		//		.Where(id => !existing.Contains(id))
		//		.ToList();

		//	if (!missing.Any())
		//	{
		//		_logger.LogInformation("Already have all requested game stats data.");
		//		return;
		//	}

		//	foreach (string gameId in missing)
		//	{
		//		if (existing.Contains(gameId))
		//		{
		//			continue;
		//		}

		//		string uri = Endpoints.Api.GameCenterStats(gameId);

		//		_logger.LogTrace($"Starting request for game {gameId} at endpoint '{uri}'.");
		//		string response = await _webRequestClient.GetStringAsync(uri, throttle: false);

		//		string filePath = _dataPath.Static.TeamGameHistoryGameStats + $"{gameId}.json";

		//		_logger.LogTrace($"Saving JSON response to '{filePath}'.");
		//		await File.WriteAllTextAsync(filePath, response);

		//		existing.Add(gameId);
		//		await _throttle.DelayAsync();

		//		_logger.LogDebug($"Finished fetching stats data for game {gameId}.");
		//	}

		//	_logger.LogDebug("Finished fetching of weekly game stats data.");
		//}

		public Task CheckHealthAsync()
		{
			// Todo:
			return Task.CompletedTask;
		}
	}
}
