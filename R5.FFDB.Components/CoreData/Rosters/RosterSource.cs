using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Http;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Rosters
{
	public interface IRosterSource : ICoreDataSource
	{
		Task FetchAsync();
	}

	public class RosterSource : IRosterSource
	{
		public string Label => "Roster";

		private ILogger<RosterSource> _logger { get; }
		private IWebRequestClient _webRequestClient { get; }
		private DataDirectoryPath _dataPath { get; }
		private IRosterScraper _scraper { get; }

		public RosterSource(
			ILogger<RosterSource> logger,
			IWebRequestClient webRequestClient,
			DataDirectoryPath dataPath,
			IRosterScraper scraper)
		{
			_logger = logger;
			_webRequestClient = webRequestClient;
			_dataPath = dataPath;
			_scraper = scraper;
		}

		public async Task FetchAsync()
		{
			_logger.LogInformation("Beginning fetching of Team Rosters.");

			List<Team> teams = TeamDataStore.GetAll();

			foreach (Team team in teams)
			{
				_logger.LogTrace($"Fetching roster page for team '{team.Abbreviation}'.");

				await FetchTeamAsync(team);

				_logger.LogDebug($"Successfully fetched roster information for team '{team.Abbreviation}'.");
			}

			_logger.LogInformation("Successfully fetched team roster pages from web.");
		}

		private async Task FetchTeamAsync(Team team)
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
			
			await File.WriteAllTextAsync(savePath, html);

			_logger.LogTrace($"Successfully saved team '{team.Abbreviation}' roster page to '{savePath}'.");
		}

		public async Task CheckHealthAsync()
		{
			var teams = TeamDataStore.GetAll();

			var testTeams = new List<Team>
			{
				teams.First(),
				teams.Last()
			};

			_logger.LogInformation($"Beginning health check for '{Label}' source. "
				+ $"Will perform checks on teams: {string.Join(", ", testTeams)}");

			foreach (var team in testTeams)
			{
				_logger.LogDebug($"Checking health using team {team}.");

				await CheckHealthForTeamAsync(team);

				_logger.LogInformation($"Health check passed for team {team}.");
			}

			_logger.LogInformation($"Health check successfully passed for '{Label}' source.");
		}

		private async Task CheckHealthForTeamAsync(Team team)
		{
			string uri = Endpoints.Page.TeamRoster(team.ShortName, team.Abbreviation);

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

			var page = new HtmlDocument();
			page.LoadHtml(html);

			_scraper.ExtractPlayers(page);
		}
	}
}
