using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R5.FFDB.Components.CoreData.Players.Models;
using R5.FFDB.Components.CoreData.WeekStats;
using R5.FFDB.Components.Http;
using R5.FFDB.Components.Resolvers;
using R5.FFDB.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Players
{
	public interface IPlayerSource : ICoreDataSource
	{
		Task FetchAsync(List<string> nflIds, bool overwriteExisting = false);
	}

	public class PlayerSource : IPlayerSource
	{
		public string Label => "Player";

		private ILogger<PlayerSource> _logger { get; }
		private DataDirectoryPath _dataPath { get; }
		private IWebRequestClient _webRequestClient { get; }
		private WebRequestThrottle _throttle { get; }
		private IWeekStatsService _weekStatsService { get; }
		private IPlayerScraper _scraper { get; }

		public PlayerSource(
			ILogger<PlayerSource> logger,
			DataDirectoryPath dataPath,
			IWebRequestClient webRequestClient,
			WebRequestThrottle throttle,
			IWeekStatsService weekStatsService,
			IPlayerScraper scraper)
		{
			_logger = logger;
			_dataPath = dataPath;
			_webRequestClient = webRequestClient;
			_throttle = throttle;
			_weekStatsService = weekStatsService;
			_scraper = scraper;
		}

		public async Task FetchAsync(List<string> nflIds, bool overwriteExisting = false)
		{
			// dont fetch teams, no profile data to fetch
			HashSet<string> teamIds = TeamDataStore.GetAll().Select(t => t.NflId).ToHashSet();

			IEnumerable<string> idsToFetch = nflIds.Where(id => !teamIds.Contains(id));
			
			if (!overwriteExisting)
			{
				HashSet<string>  existing = DirectoryFilesResolver
					.GetFileNames(_dataPath.Temp.Player, excludeExtensions: true)
					.ToHashSet();

				_logger.LogInformation($"Profile files already exist for {existing.Count} players. Will skip fetching for them. "
					+ "Clear the files first before running if you'd like to re-fetch.");

				idsToFetch = idsToFetch.Where(id => !existing.Contains(id));
			}

			idsToFetch = idsToFetch.Distinct().ToList();

			_logger.LogInformation($"Beginning fetching of profile data for {idsToFetch.Count()} player(s).");
			_logger.LogTrace($"Fetching for players (nfl ids): {string.Join(", ", idsToFetch)}");

			foreach (string id in idsToFetch)
			{
				string filePath = _dataPath.Temp.Player + $"{id}.json";
				if (File.Exists(filePath) && !overwriteExisting)
				{
					_logger.LogDebug($"Player profile file already exists for '{id}'. Will not fetch.");
					continue;
				}

				_logger.LogTrace($"Fetching player profile data for '{id}'.");

				PlayerJson playerProfile = await FetchForPlayerAsync(id);

				string serializedPlayerData = JsonConvert.SerializeObject(playerProfile);
				
				File.WriteAllText(filePath, serializedPlayerData);

				await _throttle.DelayAsync();
				_logger.LogDebug($"Finished fetching player profile for '{id}'.");
			}

			_logger.LogInformation("Finished fetching player profiles.");
		}	

		private async Task<PlayerJson> FetchForPlayerAsync(string nflId)
		{
			// the first {nflId} can be any random string, so we'll just use the id
			string uri = Endpoints.Page.PlayerProfile(nflId);

			string html;
			try
			{
				html = await _webRequestClient.GetStringAsync(uri, throttle: false);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Failed to fetch player profile page for '{nflId}' at '{uri}'.");
				throw;
			}

			var page = new HtmlDocument();
			page.LoadHtml(html);

			(string firstName, string lastName) = _scraper.ExtractNames(page);
			(int height, int weight) = _scraper.ExtractHeightWeight(page);
			DateTimeOffset dateOfBirth = _scraper.ExtractDateOfBirth(page);
			string college = _scraper.ExtractCollege(page);
			(string esbId, string gsisId) = _scraper.ExtractIds(page);
			string pictureUri = _scraper.ExtractPictureUri(page);

			return new PlayerJson
			{
				NflId = nflId,
				FirstName = firstName,
				LastName = lastName,
				EsbId = esbId,
				GsisId = gsisId,
				PictureUri = pictureUri,
				Height = height,
				Weight = weight,
				DateOfBirth = dateOfBirth.DateTime,
				College = college
			};
		}

		public async Task CheckHealthAsync()
		{
			var testPlayers = new List<string>
			{
				"2532975", // russell wilson
				"2532966" // bobby wagner
			};

			_logger.LogInformation($"Beginning health check for '{Label}' source. "
				+ $"Will perform checks on players: {string.Join(", ", testPlayers)}");

			foreach (var nflId in testPlayers)
			{
				_logger.LogDebug($"Checking health using player {nflId}.");

				await CheckHealthForPlayerAsync(nflId);

				_logger.LogDebug($"Health check passed for player {nflId}.");
			}

			_logger.LogInformation($"Health check successfully passed for '{Label}' source.");
		}

		private async Task CheckHealthForPlayerAsync(string nflId)
		{
			string uri = Endpoints.Page.PlayerProfile(nflId);

			string html;
			try
			{
				html = await _webRequestClient.GetStringAsync(uri, throttle: false);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Failed to player profile page for '{nflId}' at '{uri}'.");
				throw;
			}

			var page = new HtmlDocument();
			page.LoadHtml(html);

			try
			{
				_scraper.ExtractNames(page);
				_scraper.ExtractHeightWeight(page);
				_scraper.ExtractDateOfBirth(page);
				_scraper.ExtractCollege(page);
				_scraper.ExtractIds(page);
				_scraper.ExtractPictureUri(page);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Failed to scrape profile data for '{nflId}'.");
				throw;
			}
		}
	}
}
