using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Roster.Sources.NFLWebTeam.Models;
using R5.FFDB.Core.Data;
using R5.FFDB.Core.Models;
using R5.FFDB.Core.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Roster.Sources.NFLWebTeam
{
	public class RosterSource : IRosterSource
	{
		private ILogger<RosterSource> _logger { get; }
		private IWebRequestClient _webRequestClient { get; }

		public RosterSource(
			ILogger<RosterSource> logger,
			IWebRequestClient webRequestClient)
		{
			_logger = logger;
			_webRequestClient = webRequestClient;
		}

		public async Task<List<Core.Models.Roster>> GetAsync()
		{
			var result = new List<Core.Models.Roster>();

			List<Team> teams = Teams.Get();

			foreach (var team in teams)
			{
				var rosterInfo = await GetForTeamAsync(team);
				result.Add(rosterInfo);
			}

			return result;
		}

		public async Task<Core.Models.Roster> GetForTeamAsync(Team team)
		{
			string html = await _webRequestClient.GetStringAsync(team.RosterSourceUris[RosterSourceKeys.NFLWebTeam]);

			var page = new HtmlDocument();
			page.LoadHtml(html);

			List<RosterPlayer> players = RosterScraper.ExtractPlayers(page)
				.Select(NFLWebRosterPlayer.ToCoreEntity)
				.ToList();

			return new Core.Models.Roster
			{
				TeamId = team.Id,
				Players = players
			};
		}

		// todo: make visible to tester
		//internal async Task<Core.Models.Roster> GetForTeamAsync(Team team)
		//{
		//	return null;
		//}

		private Core.Models.Roster GetForTeam(int teamId, string rosterPage)
		{
			var page = new HtmlDocument();
			page.LoadHtml(rosterPage);

			List<RosterPlayer> players = RosterScraper.ExtractPlayers(page)
				.Select(NFLWebRosterPlayer.ToCoreEntity)
				.ToList();

			return new Core.Models.Roster
			{
				TeamId = teamId,
				Players = players
			};
		}

		public Task<bool> IsHealthyAsync()
		{
			throw new NotImplementedException();
		}
	}
}
