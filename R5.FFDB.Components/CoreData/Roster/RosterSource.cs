using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Http;
using R5.FFDB.Components.Stores;
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
			var result = new List<Core.Models.Roster>();

			List<Team> teams = TeamDataStore.GetAll();

			foreach(Team team in teams)
			{
				Core.Models.Roster roster = fromDisk
					? await GetTeamFromDiskAsync(team)
					: await GetTeamFromWebAsync(team);

				result.Add(roster);
			}

			return result;
		}

		private async Task<Core.Models.Roster> GetTeamFromWebAsync(Team team)
		{
			string uri = Endpoints.Page.TeamRoster(team.ShortName, team.Abbreviation);
			string html = await _webRequestClient.GetStringAsync(uri);

			// always save to disk on web fetch
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
			var page = new HtmlDocument();
			page.LoadHtml(pageHtml);

			List<RosterPlayer> players = RosterScraper.ExtractPlayers(page);

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
