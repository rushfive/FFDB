using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.Lib.ValueProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Rosters.Values
{
	public class RostersValue : AsyncValueProvider<List<Roster>>
	{
		private ILogger<RostersValue> _logger { get; }
		private IRosterSource _source { get; }
		private DataDirectoryPath _dataPath { get; }
		private IRosterScraper _scraper { get; }
		private ProgramOptions _programOptions { get; }

		public RostersValue(
			ILogger<RostersValue> logger,
			IRosterSource source,
			DataDirectoryPath dataPath,
			IRosterScraper scraper,
			ProgramOptions programOptions)
			: base("Rosters")
		{
			_logger = logger;
			_source = source;
			_dataPath = dataPath;
			_scraper = scraper;
			_programOptions = programOptions;
		}

		public async Task<List<string>> GetIdsAsync()
		{
			return (await GetAsync())
				.SelectMany(r => r.Players)
				.Select(p => p.NflId)
				.ToList();
		}

		public async Task<Dictionary<string, RosterPlayer>> GetPlayerMapAsync()
		{
			return (await GetAsync())
				.SelectMany(r => r.Players)
				.ToDictionary(p => p.NflId, p => p);
		}

		protected override async Task<List<Roster>> ResolveValueAsync()
		{
			if (_programOptions.SkipRosterFetch)
			{
				_logger.LogInformation("Will skip fetching of team roster pages and use the currently existing ones. "
					+ "Make sure you do have a copy of all the pages, else certain information may be ommitted from updates.");
			}
			else
			{
				await _source.FetchAsync();
			}

			return Get();
		}

		private List<Roster> Get()
		{
			_logger.LogInformation("Getting team roster information.");

			var result = new List<Roster>();

			List<Team> teams = TeamDataStore.GetAll();

			foreach (Team team in teams)
			{
				_logger.LogTrace($"Getting team roster information for'{team.Abbreviation}.'");

				Roster roster = GetTeam(team);

				result.Add(roster);

				_logger.LogDebug($"Successfully extracted team roster information for '{team.Abbreviation}'.");
			}

			_logger.LogInformation("Successfully extracted roster information for all teams.");
			return result;
		}

		private Roster GetTeam(Team team)
		{
			string pagePath = _dataPath.Temp.RosterPages + $"{team.Abbreviation}.html";
			var pageHtml = File.ReadAllText(pagePath);

			return GetForTeam(team, pageHtml);
		}

		private Roster GetForTeam(Team team, string pageHtml)
		{
			_logger.LogTrace($"Beginning scraping of team '{team.Abbreviation}' roster page.");

			var page = new HtmlDocument();
			page.LoadHtml(pageHtml);

			List<RosterPlayer> players = _scraper.ExtractPlayers(page);

			_logger.LogTrace($"Successfully scraped team '{team.Abbreviation}' roster information.");

			return new Roster
			{
				TeamId = team.Id,
				TeamAbbreviation = team.Abbreviation,
				Players = players
			};
		}
	}
}
