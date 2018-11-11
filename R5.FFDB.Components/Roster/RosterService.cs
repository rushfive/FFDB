using HtmlAgilityPack;
using R5.FFDB.Core.Data;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Roster
{
	public class RosterService
	{
		private FfdbConfig _config { get; }

		public RosterService(FfdbConfig config)
		{
			_config = config;
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
			doc.Load(@"D:\Repos\ffdb_roster\sea_roster.html");

			
			List<Core.Models.RosterPlayer> players = RosterScraper.ExtractPlayers(doc)
				.Select(p => new Core.Models.RosterPlayer
				{
					NflId = p.nflId,
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
