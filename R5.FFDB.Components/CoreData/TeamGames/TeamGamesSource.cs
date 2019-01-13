using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using R5.FFDB.Components.Http;
using R5.FFDB.Components.Resolvers;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace R5.FFDB.Components.CoreData.TeamGames
{
	public interface ITeamGamesSource : ICoreDataSource
	{
		Task FetchForWeekAsync(WeekInfo week);
		Task FetchAllAsync();
		Task FetchForWeeksAsync(List<WeekInfo> weeks);
	}

	public class TeamGamesSource : ITeamGamesSource
	{
		public string Label => "Team Game History";

		private ILogger<TeamGamesSource> _logger { get; }
		private DataDirectoryPath _dataPath { get; }
		private IWebRequestClient _webRequestClient { get; }
		private WebRequestThrottle _throttle { get; }
		private AvailableWeeksValue _availableWeeks { get; }

		public TeamGamesSource(
			ILogger<TeamGamesSource> logger,
			DataDirectoryPath dataPath,
			IWebRequestClient webRequestClient,
			WebRequestThrottle throttle,
			AvailableWeeksValue availableWeeks)
		{
			_logger = logger;
			_dataPath = dataPath;
			_webRequestClient = webRequestClient;
			_throttle = throttle;
			_availableWeeks = availableWeeks;
		}

		public async Task FetchForWeekAsync(WeekInfo week)
		{
			_logger.LogInformation($"Beginning fetching of team game history data for {week}.");

			await FetchGamesForWeekAsync(week);
			await FetchSaveGameStatsAsync(week);

			_logger.LogInformation("Finished fetching team game history data.");
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
		
		public async Task CheckHealthAsync()
		{
			var testWeeks = new List<WeekInfo>
			{
				new WeekInfo(2010, 1),
				new WeekInfo(2018, 1)
			};

			_logger.LogInformation($"Beginning health check for '{Label}' source. "
				+ $"Will perform checks on weeks: {string.Join(", ", testWeeks)}");

			foreach (var week in testWeeks)
			{
				_logger.LogDebug($"Checking health using week {week}.");

				await CheckHealthForWeekAsync(week);

				_logger.LogInformation($"Health check passed for week {week}.");
			}

			_logger.LogInformation($"Health check successfully passed for '{Label}' source.");
		}

		private async Task CheckHealthForWeekAsync(WeekInfo week)
		{
			string gamesXml = null;
			try
			{
				string uri = Endpoints.Api.ScoreStripWeekGames(week.Season, week.Week);
				gamesXml = await _webRequestClient.GetStringAsync(uri, throttle: false);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Failed to fetch games xml for week {week}.");
				throw;
			}

			string gameId = null;
			try
			{
				XElement weekGameXml = XElement.Parse(gamesXml);
				XElement gameNode = weekGameXml.Elements("gms").Single();
				
				gameId = gameNode.Elements("g").First().Attribute("eid").Value;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Failed to parse games from XML for week {week}.");
				throw;
			}

			string statsJson = null;
			try
			{
				string statsUri = Endpoints.Api.GameCenterStats(gameId);
				statsJson = await _webRequestClient.GetStringAsync(statsUri, throttle: false);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Failed to fetch game stats for game {gameId}.");
				throw;
			}
			
			try
			{
				JObject statsJObject = JObject.Parse(statsJson);

				var stats = new TeamWeekStats
				{
					TeamId = -1,
					Week = week
				};
					
				TeamGamesUtil.SetTeamWeekStats(stats, statsJObject, gameId, "home");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Failed to parse game stats for game {gameId} into expected model.");
				throw;
			}
		}
	}
}
