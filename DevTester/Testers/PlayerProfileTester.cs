using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using R5.FFDB.Components;
using R5.FFDB.Components.CoreData;
using R5.FFDB.Components.CoreData.PlayerProfile;
using R5.FFDB.Components.CoreData.Roster;
using R5.FFDB.Components.Http;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTester.Testers
{
	public interface IPlayerProfileTester
	{
		Task DownloadRosterPagesAsync();
		Task FetchSavePlayerProfilesAsync(bool downloadRosterPages);
	}

	public class PlayerProfileTester : IPlayerProfileTester
	{
		private ILogger<PlayerProfileTester> _logger { get; }
		private IWebRequestClient _webRequestClient { get; }
		private IPlayerProfileSource _playerProfileSource { get; }
		private DataDirectoryPath _dataPath { get; }
		private IRosterScraper _rosterScraper { get; }

		public PlayerProfileTester(
			ILogger<PlayerProfileTester> logger,
			IWebRequestClient webRequestClient,
			IPlayerProfileSource playerProfileSource,
			DataDirectoryPath dataPath,
			IRosterScraper rosterScraper)
		{
			_logger = logger;
			_webRequestClient = webRequestClient;
			_playerProfileSource = playerProfileSource;
			_dataPath = dataPath;
			_rosterScraper = rosterScraper;
		}

		public async Task DownloadRosterPagesAsync()
		{
			List<Team> teams = TeamDataStore.GetAll();

			foreach (Team team in teams)
			{
				string uri = Endpoints.Page.TeamRoster(team.ShortName, team.Abbreviation);
				string page = await _webRequestClient.GetStringAsync(uri, throttle: true);
				await File.WriteAllTextAsync(_dataPath.Temp.RosterPages + $"{team.Abbreviation}.html", page);
			}
		}

		public async Task FetchSavePlayerProfilesAsync(bool downloadRosterPages)
		{
			List<Roster> rosters = GetRosters();

			//List<string> nflIds = rosters
			//	.SelectMany(r => r.Players)
			//	.Select(p => p.NflId)
			//	.Distinct()
			//	.ToList();

			List<string> playerIds = rosters
				.SelectMany(r => r.Players)
				.Select(p => p.NflId)
				.Distinct()
				.ToList();

			_logger.LogDebug($"Found '{playerIds.Count}' players to fetch profile data for.");

			//await _playerProfileSource.FetchAndSaveAsync();

			_logger.LogDebug("Finished fetching player profile data by rosters.");
		}

		private List<Roster> GetRosters()
		{
			var rosters = new List<Roster>();

			List<Team> teams = TeamDataStore.GetAll();//.GetRange(0, 1);

			foreach (var team in teams)
			{
				string pagePath = _dataPath.Temp.RosterPages + $"{team.Abbreviation}.html";
				var pageHtml = File.ReadAllText(pagePath);

				var page = new HtmlDocument();
				page.LoadHtml(pageHtml);

				List<RosterPlayer> players = _rosterScraper.ExtractPlayers(page);

				rosters.Add(new Roster
				{
					TeamId = team.Id,
					TeamAbbreviation = team.Abbreviation,
					Players = players
				});
			}

			return rosters;
		}
	}
}
