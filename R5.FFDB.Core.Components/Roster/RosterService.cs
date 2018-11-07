using HtmlAgilityPack;
using R5.FFDB.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Core.Components.Roster
{
	public class RosterService
	{
		private FfdbConfig _config { get; }

		public RosterService(FfdbConfig config)
		{
			_config = config;
		}

		public async Task<List<Core.Game.Roster>> GetAsync()
		{
			var result = new List<Core.Game.Roster>();

			List<Game.Team> teams = Teams.Get();

			foreach (var team in teams)
			{
				var rosterInfo = await GetForTeamAsync(team);
				result.Add(rosterInfo);
			}

			return result;
		}

		public async Task<Core.Game.Roster> GetForTeamAsync(Game.Team team)
		{
			// UNCOMMENT later
			//var web = new HtmlWeb();
			//HtmlDocument page = await web.LoadFromWebAsync(team.RosterPageUri);


			// mock getting from endpoint
			var doc = new HtmlDocument();
			doc.Load(@"D:\Repos\ffdb_roster\sea_roster.html");

			
			List<Core.Game.RosterPlayer> players = RosterScraper.ExtractPlayers(doc)
				.Select(p => new Game.RosterPlayer
				{
					NflId = p.nflId,
					Position = p.position,
					Status = p.status
				})
				.ToList();

			return new Game.Roster
			{
				TeamId = team.Id,
				Players = players
			};
		}
	}
}
