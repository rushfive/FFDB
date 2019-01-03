using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R5.FFDB.Components.CoreData.PlayerProfile.Models;
using R5.FFDB.Components.CoreData.Roster;
using R5.FFDB.Components.CoreData.TeamData.Models;
using R5.FFDB.Components.CoreData.WeekStats;
using R5.FFDB.Components.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.PlayerProfile
{
	public interface IPlayerProfileSource : ICoreDataSource
	{
		Task FetchAsync(List<string> nIds);
	}

	public class PlayerProfileSource : IPlayerProfileSource
	{
		public string Label => "Player Profile";

		private ILogger<PlayerProfileSource> _logger { get; }
		private DataDirectoryPath _dataPath { get; }
		private IWebRequestClient _webRequestClient { get; }
		private WebRequestThrottle _throttle { get; }

		private IWeekStatsService _weekStatsService { get; }
		private IRosterService _rosterService { get; }

		public PlayerProfileSource(
			ILogger<PlayerProfileSource> logger,
			DataDirectoryPath dataPath,
			IWebRequestClient webRequestClient,
			WebRequestThrottle throttle,
			IWeekStatsService weekStatsService,
			IRosterService rosterService)
		{
			_logger = logger;
			_dataPath = dataPath;
			_webRequestClient = webRequestClient;
			_throttle = throttle;
			_weekStatsService = weekStatsService;
			_rosterService = rosterService;
		}

		public async Task FetchAsync(List<string> nIds)
		{
			// skip teams, no profile data to fetch
			var teamIds = TeamDataStore.GetAll().Select(t => t.NflId);
			nIds = nIds.Where(id => !teamIds.Contains(id)).Distinct().ToList();

			_logger.LogInformation($"Beginning fetching of profile data for {nIds.Count} player(s).");
			_logger.LogTrace($"Fetching for players (nfl ids): {string.Join(", ", nIds)}");

			foreach (string id in nIds)
			{
				_logger.LogTrace($"Fetching player profile data for '{id}'.");

				PlayerProfileJson playerProfile = await FetchForPlayerAsync(id);

				string serializedPlayerData = JsonConvert.SerializeObject(playerProfile);

				string path = _dataPath.Static.PlayerProfile + $"{id}.json";
				File.WriteAllText(path, serializedPlayerData);

				await _throttle.DelayAsync();
				_logger.LogDebug($"Finished fetching player profile for '{id}'.");
			}

			_logger.LogInformation("Finished fetching player profiles.");
		}


		// pre per-week below
		
		public async Task FetchAndSaveAsync()
		{
			_logger.LogInformation("Beginning fetching of player profiles based on ids found from team rosters and week stats.");

			List<Core.Models.Roster> rosters = _rosterService.Get();
			List<Core.Models.WeekStats> weekStats = _weekStatsService.Get();

			List<string> nflIds = rosters
				.SelectMany(r => r.Players)
				.Select(p => p.NflId)
				.Concat(weekStats.SelectMany(ws => ws.Players).Select(p => p.NflId))
				.ToList();

			HashSet<string> existing = GetPlayersWithExistingProfileData();

			// skip teams, no profile data to fetch
			TeamDataStore.GetAll().ForEach(t => existing.Add(t.NflId));

			List<string> newPlayers = nflIds
				.Where(id => !existing.Contains(id))
				.Distinct()
				.ToList();

			if (!newPlayers.Any())
			{
				_logger.LogInformation("Already have player profile data for all - no fetching necessary.");
				return;
			}

			int alreadyExistingCount = nflIds.Count - newPlayers.Count;
			if (alreadyExistingCount > 0)
			{
				_logger.LogInformation($"Already have profile data for {nflIds.Count - newPlayers.Count} players.");
			}

			int remaining = newPlayers.Count;
			foreach (string nflId in newPlayers)
			{
				_logger.LogTrace($"Fetching profile data for '{nflId}'.");

				PlayerProfileJson playerProfile = null;
				try
				{
					playerProfile = await FetchForPlayerAsync(nflId);

					string serializedPlayerData = JsonConvert.SerializeObject(playerProfile);

					string path = _dataPath.Static.PlayerProfile + $"{nflId}.json";
					File.WriteAllText(path, serializedPlayerData);

					_logger.LogDebug($"Successfully fetched profile data for '{nflId}' ({playerProfile.FirstName} {playerProfile.LastName}) "
						+ $"(remaining: {remaining - 1})");
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, $"Failed to fetch player profile for '{nflId}': {ex.Message}. Check the player_profile_fetch file error logs for more information.");
				}
				finally
				{
					remaining--;
				}

				await _throttle.DelayAsync();
			}
		}

		private HashSet<string> GetPlayersWithExistingProfileData()
		{
			var directory = new DirectoryInfo(_dataPath.Static.PlayerProfile);

			var existing = directory
				.GetFiles()
				.Select(f =>
				{
					string fileName = f.Name;
					var split = fileName.Split(".");
					return split[0];
				})
				.ToHashSet();

			// skip teams, no profile data to fetch
			TeamDataStore.GetAll().ForEach(t => existing.Add(t.NflId));

			return existing;
		}

		private async Task<PlayerProfileJson> FetchForPlayerAsync(string nflId)
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
				_logger.LogError(ex, $"Failed to player profile page for '{nflId}' at '{uri}'.");
				throw;
			}

			var page = new HtmlDocument();
			page.LoadHtml(html);

			(string firstName, string lastName) = PlayerProfileScraper.ExtractNames(page);
			(int height, int weight) = PlayerProfileScraper.ExtractHeightWeight(page);
			DateTimeOffset dateOfBirth = PlayerProfileScraper.ExtractDateOfBirth(page);
			string college = PlayerProfileScraper.ExtractCollege(page);
			(string esbId, string gsisId) = PlayerProfileScraper.ExtractIds(page);
			string pictureUri = PlayerProfileScraper.ExtractPictureUri(page);

			return new PlayerProfileJson
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

		public Task CheckHealthAsync()
		{
			// Todo:
			return Task.CompletedTask;
		}
	}
}
