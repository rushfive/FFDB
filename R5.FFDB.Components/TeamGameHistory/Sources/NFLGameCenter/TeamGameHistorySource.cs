using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Resolvers;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace R5.FFDB.Components.TeamGameHistory.Sources.NFLGameCenter
{
	public class TeamGameHistorySource : ITeamGameHistorySource
	{
		private ILogger<TeamGameHistorySource> _logger { get; }
		private DataDirectoryPath _dataPath { get; }
		private IWebRequestClient _webRequestClient { get; }
		private WebRequestThrottle _throttle { get; }
		private IAvailableWeeksResolver _availableWeeks { get; }

		public TeamGameHistorySource(
			ILogger<TeamGameHistorySource> logger,
			DataDirectoryPath dataPath,
			IWebRequestClient webRequestClient,
			WebRequestThrottle throttle,
			IAvailableWeeksResolver availableWeeks)
		{
			_logger = logger;
			_dataPath = dataPath;
			_webRequestClient = webRequestClient;
			_throttle = throttle;
			_availableWeeks = availableWeeks;
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

			foreach(string gameId in GetAllAvailableGameIds())
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

		private List<string> GetAllAvailableGameIds()
		{
			var result = new List<string>();

			var weekGameFiles = DirectoryFilesResolver.GetFileNames(_dataPath.Static.TeamGameHistoryWeekGames);

			foreach(string file in weekGameFiles)
			{
				XElement weekGameXml = XElement.Load(file);

				IEnumerable<string> gameIds = weekGameXml
					.Elements("gms")
					.Single()
					.Elements("g")
					.Select(g => g.Attribute("eid").Value);

				result.AddRange(gameIds);
			}

			return result;
		}

		public Task<bool> IsHealthyAsync()
		{
			throw new NotImplementedException();
		}
	}
}
