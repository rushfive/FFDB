using HtmlAgilityPack;
using R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1.Mappers
{
	public interface IToVersionedModelMapper : IMapper<string, RosterVersionedModel>
	{

	}
	public class ToVersionedModelMapper : IToVersionedModelMapper
	{
		private IRosterScraper _scraper { get; }

		public ToVersionedModelMapper(IRosterScraper scraper)
		{
			_scraper = scraper;
		}

		public RosterVersionedModel Map(string httpResponse)
		{
			var page = new HtmlDocument();
			page.LoadHtml(httpResponse);

			List<RosterVersionedModel.Player> players = _scraper.ExtractPlayers(page);

			return new RosterVersionedModel
			{
				Players = players
			};
		}
	}
}
