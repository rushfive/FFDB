using HtmlAgilityPack;
using R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1.Models;
using R5.FFDB.Core;
using R5.FFDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1.Mappers
{
	public interface IToVersionedMapper : IAsyncMapper<string, RosterVersioned, Team> { }

	public class ToVersionedModelMapper : IToVersionedMapper
	{
		private IAppLogger _logger { get; }
		private IRosterScraper _scraper { get; }

		public ToVersionedModelMapper(
			IAppLogger logger,
			IRosterScraper scraper)
		{
			_logger = logger;
			_scraper = scraper;
		}

		public Task<RosterVersioned> MapAsync(string httpResponse, Team team)
		{
			var page = new HtmlDocument();
			page.LoadHtml(httpResponse);

			List<RosterVersioned.Player> players = _scraper.ExtractPlayers(page);

			var failedPlayers = players.Where(p => p.NflId == null).ToList();
			if (failedPlayers.Any())
			{
				_logger.LogInformation($"Failed to scrape necessary data for {failedPlayers.Count} players. "
					+ $"Will skip adding to team '{team}' roster. Failed players:"
					+ Environment.NewLine + "{@FailedPlayers}", failedPlayers);
			}

			return Task.FromResult(new RosterVersioned
			{
				TeamId = team.Id,
				TeamAbbreviation = team.Abbreviation,
				Players = players.Where(p => p.NflId != null).ToList()
			});
		}
	}
}
