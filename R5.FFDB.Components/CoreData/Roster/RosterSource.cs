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

		public Task CheckHealthAsync()
		{
			// Todo:
			return Task.CompletedTask;
		}
	}
}
