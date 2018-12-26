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
		Task FetchAndSaveAsync();
		List<Core.Models.Roster> Get();
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

		public async Task FetchAndSaveAsync()
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

		public List<Core.Models.Roster> Get()
		{
			_logger.LogInformation("Getting team roster information.");

			var result = new List<Core.Models.Roster>();

			List<Team> teams = TeamDataStore.GetAll();

			foreach(Team team in teams)
			{
				_logger.LogTrace($"Getting team roster information for'{team.Abbreviation}.'");

				Core.Models.Roster roster = GetTeam(team);

				result.Add(roster);

				_logger.LogDebug($"Successfully extracted team roster information for '{team.Abbreviation}'.");
			}

			_logger.LogInformation("Successfully extracted roster information for all teams.");
			return result;
		}

		

		private Core.Models.Roster GetTeam(Team team)
		{
			string pagePath = _dataPath.Temp.RosterPages + $"{team.Abbreviation}.html";
			var pageHtml = File.ReadAllText(pagePath);

			return GetForTeam(team, pageHtml);
		}

		private Core.Models.Roster GetForTeam(Team team, string pageHtml)
		{
			_logger.LogTrace($"Beginning scraping of team '{team.Abbreviation}' roster page.");

			var page = new HtmlDocument();
			page.LoadHtml(pageHtml);

			List<RosterPlayer> players = _scraper.ExtractPlayers(page);

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
