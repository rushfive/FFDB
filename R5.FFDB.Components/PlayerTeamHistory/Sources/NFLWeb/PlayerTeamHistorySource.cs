﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R5.FFDB.Components.ErrorFileLog;
using R5.FFDB.Components.PlayerTeamHistory.Sources.NFLWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.PlayerTeamHistory.Sources.NFLWeb
{
	public class PlayerTeamHistorySource : IPlayerTeamHistorySource
	{
		private ILogger<PlayerTeamHistorySource> _logger { get; }
		private DataDirectoryPath _dataPath { get; }
		private IWebRequestClient _webRequestClient { get; }
		private WebRequestThrottle _throttle { get; }
		private IErrorFileLogger _errorFileLogger { get; }

		public PlayerTeamHistorySource(
			ILogger<PlayerTeamHistorySource> logger,
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

		public async Task FetchAndSaveAsync(List<Core.Models.PlayerProfile> players)
		{
			// skip teams, they have a different history/data type
			HashSet<string> teamIds = TeamDataStore.GetAll()
				.Select(t => t.NflId)
				.ToHashSet();

			List<Core.Models.PlayerProfile> fetchPlayers = players
				.Where(p => !teamIds.Contains(p.NflId))
				.ToList();

			int remaining = fetchPlayers.Count;
			_logger.LogInformation($"Fetching team histories for '{fetchPlayers.Count}' players.");

			foreach(Core.Models.PlayerProfile player in fetchPlayers)
			{
				_logger.LogTrace($"Fetching team history for '{player.NflId}'.");

				PlayerTeamHistoryJson history = null;
				try
				{
					history = await FetchAsync(player.NflId);

					string serializedHistory = JsonConvert.SerializeObject(history);

					string path = _dataPath.Static.PlayerTeamHistory + $"{player.NflId}.json";
					File.WriteAllText(path, serializedHistory);

					_logger.LogDebug($"Successfully fetched team history for '{player.NflId}' ({player.FirstName} {player.LastName}) "
						+ $"(remaining: {remaining - 1})");
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, $"Failed to fetch team history for '{player.NflId}': {ex.Message}. Check the player_team_history file error logs for more information.");
					_errorFileLogger.LogPlayerTeamHistoryFetchError(player.NflId, ex);
				}
				finally
				{
					remaining--;
				}
			}
		}

		private async Task<PlayerTeamHistoryJson> FetchAsync(string nflId)
		{
			var result = new PlayerTeamHistoryJson
			{
				NflId = nflId,
				SeasonWeekTeamMap = new Dictionary<int, Dictionary<int, int>>()
			};

			List<int> seasons = await GetSeasonsPlayedAsync(nflId);
			_logger.LogDebug($"Player '{nflId}' has played in seasons: {string.Join(", ", seasons)}. Beginning fetch.");

			foreach(int season in seasons)
			{
				_logger.LogTrace($"Fetching team history for '{nflId}' season {season}");

				Dictionary<int, int> history = await GetHistoryBySeasonAsync(nflId, season);
				result.SeasonWeekTeamMap[season] = history;

				_logger.LogDebug($"Successfully fetched team history for '{nflId}' season {season}.");
			}

			_logger.LogInformation($"Successfully fetched team history for player '{nflId}'.");
			return result;
		}

		private async Task<List<int>> GetSeasonsPlayedAsync(string nflId)
		{

		}

		private async Task<Dictionary<int, int>> GetHistoryBySeasonAsync(string nflId, int season)
		{

		}

		public Task<bool> IsHealthyAsync()
		{
			return Task.FromResult(true);
		}
	}
}
