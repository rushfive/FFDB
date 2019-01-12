using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Roster.Values
{
	public class RostersValue : AsyncValueProvider<List<Core.Models.Roster>>
	{
		private ILogger<RostersValue> _logger { get; }
		private IRosterSource _source { get; }
		private DataDirectoryPath _dataPath { get; }
		private IRosterScraper _scraper { get; }

		public RostersValue(
			ILogger<RostersValue> logger,
			IRosterSource source,
			DataDirectoryPath dataPath,
			IRosterScraper scraper)
			: base("Rosters")
		{
			_logger = logger;
			_source = source;
			_dataPath = dataPath;
			_scraper = scraper;
		}

		protected override async Task<List<Core.Models.Roster>> ResolveValueAsync()
		{
			//await _source.FetchAsync();

			return Get();
		}

		private List<Core.Models.Roster> Get()
		{
			_logger.LogInformation("Getting team roster information.");

			var result = new List<Core.Models.Roster>();

			List<Team> teams = TeamDataStore.GetAll();

			foreach (Team team in teams)
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
	}
}
