using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R5.FFDB.Components.ErrorFileLog;
using R5.FFDB.Components.PlayerProfile.Sources.NFLWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.PlayerProfile.Sources.NFLWeb
{
	public class PlayerProfileSource : IPlayerProfileSource
	{
		private ILogger<PlayerProfileSource> _logger { get; }
		private DataDirectoryPath _dataPath { get; }
		private IWebRequestClient _webRequestClient { get; }
		private WebRequestThrottle _throttle { get; }
		private IErrorFileLogger _errorFileLogger { get; }

		public PlayerProfileSource(
			ILogger<PlayerProfileSource> logger,
			DataDirectoryPath dataPath,
			IWebRequestClient webRequestClient,
			WebRequestThrottle throttle,
			IErrorFileLogger errorFileLogger)
		{
			_logger = logger;
			_dataPath = dataPath;
			_webRequestClient = webRequestClient;
			_throttle = throttle;
			_errorFileLogger = errorFileLogger;
		}

		public Core.Models.PlayerProfile GetPlayerProfile(string nflId)
		{
			string path = _dataPath.Static.PlayerData + $"{nflId}.json";
			PlayerProfileJson playerData = JsonConvert.DeserializeObject<PlayerProfileJson>(File.ReadAllText(path));
			return PlayerProfileJson.ToCoreEntity(playerData);
		}

		public List<Core.Models.PlayerProfile> GetPlayerProfile(List<string> nflIds)
		{
			return nflIds
				.Select(id => GetPlayerProfile(id))
				.ToList();
		}

		// ensures ALREADY EXISTING players ARENT fetched again
		public async Task FetchAndSavePlayerDataFilesAsync(List<string> playerNflIds)
		{
			HashSet<string> existing = GetExistingPlayerNflIds();

			List<string> newPlayers = playerNflIds
				.Where(id => !existing.Contains(id))
				.Distinct()
				.ToList();

			if (!newPlayers.Any())
			{
				_logger.LogInformation("Already have player profile data for all - no fetching necessary.");
				return;
			}

			int alreadyExistingCount = playerNflIds.Count - newPlayers.Count;
			if (alreadyExistingCount > 0)
			{
				_logger.LogInformation($"Already have profile data for {playerNflIds.Count - newPlayers.Count} players.");
			}

			int remaining = newPlayers.Count;
			foreach (string nflId in newPlayers)
			{
				//if (existing.Contains(nflId))
				//{
				//	_logger.LogDebug($"Skipping fetching of profile data for player '{nflId}' because it already exists "
				//		+ $"(remaining: {--remaining})");
				//	continue;
				//}

				_logger.LogTrace($"Fetching profile data for '{nflId}'.");

				// no longer needed since we get this info from rosters
				//NgsContentPlayer ngsContent = await GetNgsContentInfoAsync(id);

				//NflPlayerProfile nflProfile = await GetNflPlayerProfileInfoAsync(nflId);

				//var playerData = new PlayerProfileJson
				//{
				//	NflId = nflId,
				//	EsbId = nflProfile.EsbId,
				//	GsisId = nflProfile.GsisId,
				//	PictureUri = nflProfile.PictureUri,
				//	FirstName = nflProfile.FirstName,
				//	LastName = nflProfile.LastName,
				//	Height = nflProfile.Height,
				//	Weight = nflProfile.Weight,
				//	DateOfBirth = nflProfile.DateOfBirth.DateTime,
				//	College = nflProfile.College
				//};

				PlayerProfileJson playerProfile = null;
				try
				{
					playerProfile = await fetchPlayerAsync(nflId);

					string serializedPlayerData = JsonConvert.SerializeObject(playerProfile);

					string path = _dataPath.Static.PlayerData + $"{nflId}.json";
					File.WriteAllText(path, serializedPlayerData);

					_logger.LogDebug($"Successfully fetched profile data for '{nflId}' ({playerProfile.FirstName} {playerProfile.LastName}) "
					+ $"(remaining: {--remaining})");
				}
				catch (Exception ex)
				{
					_errorFileLogger.LogPlayerProfileFetchError(nflId, ex);
				}
				
				//existing.Add(nflId);
				
				await Task.Delay(_throttle.Get());
			}

			// local functions
			async Task<PlayerProfileJson> fetchPlayerAsync(string id)
			{
				NflPlayerProfile nflProfile = await GetNflPlayerProfileInfoAsync(id);

				return new PlayerProfileJson
				{
					NflId = id,
					EsbId = nflProfile.EsbId,
					GsisId = nflProfile.GsisId,
					PictureUri = nflProfile.PictureUri,
					FirstName = nflProfile.FirstName,
					LastName = nflProfile.LastName,
					Height = nflProfile.Height,
					Weight = nflProfile.Weight,
					DateOfBirth = nflProfile.DateOfBirth.DateTime,
					College = nflProfile.College
				};
			}
		}

		private HashSet<string> GetExistingPlayerNflIds()
		{
			var directory = new DirectoryInfo(_dataPath.Static.PlayerData);

			return directory
				.GetFiles()
				.Select(f =>
				{
					string fileName = f.Name;
					var split = fileName.Split(".");
					return split[0];
				})
				.ToHashSet();
		}

		private async Task<NgsContentPlayer> GetNgsContentInfoAsync(string nflId)
		{
			string uri = $"http://api.fantasy.nfl.com/v2/player/ngs-content?playerId={nflId}";

			string response;
			try
			{
				response = await _webRequestClient.GetStringAsync(uri, throttle: false);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Failed to fetch data for '{nflId}' at '{uri}'.");
				throw;
			}

			try
			{
				var json = JsonConvert.DeserializeObject<NgsContentJson>(response);
				return NgsContentJson.ToEntity(json);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Failed to deserialize fetched data for '{nflId}'.", response);
				throw;
			}
		}

		private async Task<NflPlayerProfile> GetNflPlayerProfileInfoAsync(string nflId)
		{
			//string name = firstName.ToLower() + lastName?.ToLower() ?? "";

			// the first {nflId} can be any random string, so we'll just use the id
			string uri = $"http://www.nfl.com/player/{nflId}/{nflId}/profile";

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
			//string html = await _webRequestClient.GetStringAsync(uri);

			var page = new HtmlDocument();
			page.LoadHtml(html);

			//int number = PlayerProfileScraper.ExtractPlayerNumber(page);
			(string firstName, string lastName) = PlayerProfileScraper.ExtractNames(page);
			(int height, int weight) = PlayerProfileScraper.ExtractHeightWeight(page);
			DateTimeOffset dateOfBirth = PlayerProfileScraper.ExtractDateOfBirth(page);
			string college = PlayerProfileScraper.ExtractCollege(page);
			(string esbId, string gsisId) = PlayerProfileScraper.ExtractIds(page);
			string pictureUri = PlayerProfileScraper.ExtractPictureUri(page);

			return new NflPlayerProfile
			{
				FirstName = firstName,
				LastName = lastName,
				EsbId = esbId,
				GsisId = gsisId,
				PictureUri = pictureUri,
				//Number = number,
				Height = height,
				Weight = weight,
				DateOfBirth = dateOfBirth,
				College = college
			};
		}

		public Task<bool> IsHealthyAsync()
		{
			// todo:
			return Task.FromResult(true);
		}
	}
}
