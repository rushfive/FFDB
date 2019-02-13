using HtmlAgilityPack;
using R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Update.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Update.Mappers
{
	public interface IToVersionedMapper : IAsyncMapper<string, PlayerUpdateVersioned, string> { }

	public class ToVersionedMapper : IToVersionedMapper
	{
		private IPlayerScraper _scraper { get; }

		public ToVersionedMapper(IPlayerScraper scraper)
		{
			_scraper = scraper;
		}

		public Task<PlayerUpdateVersioned> MapAsync(string httpResponse, string nflId)
		{
			var page = new HtmlDocument();
			page.LoadHtml(httpResponse);

			(string firstName, string lastName) = _scraper.ExtractNames(page);

			return Task.FromResult(new PlayerUpdateVersioned
			{
				FirstName = firstName,
				LastName = lastName
			});
		}
	}
}
