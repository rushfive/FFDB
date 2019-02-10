using HtmlAgilityPack;
using R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1.Models;
using R5.FFDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1.Mappers
{
	public interface IToVersionedModelMapper : IAsyncMapper<string, RosterVersioned, Team> { }

	public class ToVersionedModelMapper : IToVersionedModelMapper
	{
		private IRosterScraper _scraper { get; }

		public ToVersionedModelMapper(IRosterScraper scraper)
		{
			_scraper = scraper;
		}

		public Task<RosterVersioned> MapAsync(string httpResponse, Team team)
		{
			var page = new HtmlDocument();
			page.LoadHtml(httpResponse);

			List<RosterVersioned.Player> players = _scraper.ExtractPlayers(page);

			return Task.FromResult(new RosterVersioned
			{
				TeamId = team.Id,
				TeamAbbreviation = team.Abbreviation,
				Players = players
			});
		}
	}
}
