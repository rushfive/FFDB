using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.TeamData.Models;
using R5.FFDB.Components.Http;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Roster
{
	public interface IRosterSource : ICoreDataSource
	{
		Task<List<Core.Models.Roster>> GetAsync(bool fromDisk = false);
	}

	public class RosterSource : IRosterSource
	{
		public string Label => "Roster";

		private ILogger<RosterSource> _logger { get; }
		private IWebRequestClient _webRequestClient { get; }
		private DataDirectoryPath _dataPath { get; }

		public RosterSource(
			ILogger<RosterSource> logger,
			IWebRequestClient webRequestClient,
			DataDirectoryPath dataPath)
		{
			_logger = logger;
			_webRequestClient = webRequestClient;
			_dataPath = dataPath;
		}

		public async Task<List<Core.Models.Roster>> GetAsync(bool fromDisk = false)
		{
			_logger.LogInformation("Beginning fetching of Team Rosters.");

			var result = new List<Core.Models.Roster>();

			List<Team> teams = TeamDataStore.GetAll();

			foreach(Team team in teams)
			{
				_logger.LogDebug($"Fetching roster for team '{team.Abbreviation}'"
					+ (fromDisk ? " from disk." : "."));

				Core.Models.Roster roster = fromDisk
					? await GetTeamFromDiskAsync(team)
					: await GetTeamFromWebAsync(team);

				result.Add(roster);

				_logger.LogDebug($"Successfully fetched roster information for team '{team.Abbreviation}'.");
			}

			_logger.LogInformation("Successfully fetched Team Roster information.");
			return result;
		}

		private async Task<Core.Models.Roster> GetTeamFromWebAsync(Team team)
		{
			string uri = Endpoints.Page.TeamRoster(team.ShortName, team.Abbreviation);
			_logger.LogTrace($"Beginning request for team '{team.Abbreviation}' roster page at '{uri}'.");

			string html = null;
			try
			{
				html = await _webRequestClient.GetStringAsync(uri);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Failed to fetch team '{team.Abbreviation}' roster page at '{uri}'.");
				throw;
			}

			// always save to disk on web fetch
			string savePath = _dataPath.Temp.RosterPages + $"{team.Abbreviation}.html";

			_logger.LogTrace($"Saving team '{team.Abbreviation}' roster page to '{savePath}'.");
			await File.WriteAllTextAsync(_dataPath.Temp.RosterPages + $"{team.Abbreviation}.html", html);

			return GetForTeam(team, html);
		}

		private async Task<Core.Models.Roster> GetTeamFromDiskAsync(Team team)
		{
			string pagePath = _dataPath.Temp.RosterPages + $"{team.Abbreviation}.html";
			var pageHtml = await File.ReadAllTextAsync(pagePath);

			return GetForTeam(team, pageHtml);
		}

		private Core.Models.Roster GetForTeam(Team team, string pageHtml)
		{
			_logger.LogTrace($"Beginning scraping of team '{team.Abbreviation}' roster page.");

			var page = new HtmlDocument();
			page.LoadHtml(pageHtml);

			List<RosterPlayer> players = RosterScraper.ExtractPlayers(page);

			_logger.LogTrace($"Successfully scraped team '{team.Abbreviation}' roster information.");

			return new Core.Models.Roster
			{
				TeamId = team.Id,
				TeamAbbreviation = team.Abbreviation,
				Players = players
			};
		}

		public Task CheckHealthAsync()
		{
			// Todo:
			return Task.CompletedTask;
		}
	}
}
