using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using R5.FFDB.Core.Data;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Roster
{
	public interface IRosterService
	{
		Task<List<Core.Models.Roster>> GetAsync();
	}

	public class RosterService : IRosterService
	{
		private ILogger<RosterService> _logger { get; }

		public RosterService(ILogger<RosterService> logger)
		{
			_logger = logger;
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
			// UNCOMMENT later
			// --- FIX: should use webReqClient to get html string
			//var web = new HtmlWeb();
			//HtmlDocument page = await web.LoadFromWebAsync(team.RosterPageUri);


			// mock getting from endpoint
			var doc = new HtmlDocument();
			doc.Load(@"D:\Repos\ffdb_stuff\misc_stuff\sea_roster.html");

			
			List<RosterPlayer> players = RosterScraper.ExtractPlayers(doc)
				.Select(p => new RosterPlayer
				{
					NflId = p.nflId,
					Number = p.number,
					Position = p.position,
					Status = p.status
				})
				.ToList();

			return new Core.Models.Roster
			{
				TeamId = team.Id,
				Players = players
			};
		}
	}
}
