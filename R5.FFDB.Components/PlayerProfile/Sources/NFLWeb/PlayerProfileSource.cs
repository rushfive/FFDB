﻿using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R5.FFDB.Components.ErrorFileLog;
using R5.FFDB.Components.PlayerProfile.Sources.NFLWeb.Models;
using R5.FFDB.Components.Stores;
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

		public List<Core.Models.PlayerProfile> GetAll()
		{
			var directory = new DirectoryInfo(_dataPath.Static.PlayerProfile);

			return directory
				.GetFiles()
				.Select(f =>
				{
					string filePath = f.ToString();
					PlayerProfileJson json = JsonConvert.DeserializeObject<PlayerProfileJson>(File.ReadAllText(filePath));
					return PlayerProfileJson.ToCoreEntity(json);
				})
				.ToList();
		}

		public Core.Models.PlayerProfile GetPlayerProfile(string nflId)
		{
			string path = _dataPath.Static.PlayerProfile + $"{nflId}.json";
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
		public async Task FetchAndSaveAsync(List<string> nflIds)
		{
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

				// no longer needed since we get this info from rosters
				//NgsContentPlayer ngsContent = await GetNgsContentInfoAsync(id);

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
					_errorFileLogger.LogPlayerProfileFetchError(nflId, ex);
				}
				finally
				{
					remaining--;
				}
				
				await Task.Delay(_throttle.Get());
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
			string uri = $"http://www.nfl.com/player/{nflId}/{nflId}/profile"; // change this to the gamelogs page, it contains profile stuff too

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

		public Task<bool> IsHealthyAsync()
		{
			// todo:
			return Task.FromResult(true);
		}
	}
}