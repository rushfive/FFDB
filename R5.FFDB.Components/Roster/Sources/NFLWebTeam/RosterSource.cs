using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Roster.Sources.NFLWebTeam.Models;
using R5.FFDB.Core.Data;
using R5.FFDB.Core.Models;
using R5.FFDB.Core.Sources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Roster.Sources.NFLWebTeam
{
	public class RosterSource : IRosterSource
	{
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

		public async Task<List<Core.Models.Roster>> GetFromWebAsync(bool saveToDisk = true)
		{
			var result = new List<Core.Models.Roster>();

			List<Team> teams = Teams.Get();

			foreach (Team team in teams)
			{
				string html = await _webRequestClient.GetStringAsync(team.RosterSourceUris[RosterSourceKeys.NFLWebTeam]);

				if (saveToDisk)
				{
					await File.WriteAllTextAsync(_dataPath.RosterPages + $"{team.Abbreviation}.html", html);
				}

				Core.Models.Roster rosterInfo = GetForTeam(team, html);
				result.Add(rosterInfo);
			}

			return result;
		}

		public List<Core.Models.Roster> GetFromDisk()
		{
			var result = new List<Core.Models.Roster>();

			List<Team> teams = Teams.Get();

			foreach (Team team in teams)
			{
				string pagePath = _dataPath.RosterPages + $"{team.Abbreviation}.html";
				var pageHtml = File.ReadAllText(pagePath);

				Core.Models.Roster rosterInfo = GetForTeam(team, pageHtml);
				result.Add(rosterInfo);
			}

			return result;
		}

		private Core.Models.Roster GetForTeam(Team team, string pageHtml)
		{
			var page = new HtmlDocument();
			page.LoadHtml(pageHtml);

			List<RosterPlayer> players = RosterScraper.ExtractPlayers(page)
				.Select(NFLWebRosterPlayer.ToCoreEntity)
				.ToList();

			return new Core.Models.Roster
			{
				TeamId = team.Id,
				TeamAbbreviation = team.Abbreviation,
				Players = players
			};
		}

		//private Core.Models.Roster GetForTeam(int teamId, string rosterPage)
		//{
		//	var page = new HtmlDocument();
		//	page.LoadHtml(rosterPage);

		//	List<RosterPlayer> players = RosterScraper.ExtractPlayers(page)
		//		.Select(NFLWebRosterPlayer.ToCoreEntity)
		//		.ToList();

		//	return new Core.Models.Roster
		//	{
		//		TeamId = teamId,
		//		Players = players
		//	};
		//}

		public Task<bool> IsHealthyAsync()
		{
			// todo:
			return Task.FromResult(true);
		}
	}
}
