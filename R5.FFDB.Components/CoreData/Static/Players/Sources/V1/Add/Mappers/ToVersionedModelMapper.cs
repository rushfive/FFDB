using HtmlAgilityPack;
using R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Add.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Add.Mappers
{
	// instantiate using serviceProvider so we can resolve the scraper
	// containing the logger
	public class ToVersionedModelMapper : IMapper<string, PlayerAddVersionedModel>
	{
		private IPlayerScraper _scraper { get; }

		public ToVersionedModelMapper(IPlayerScraper scraper)
		{
			_scraper = scraper;
		}

		public PlayerAddVersionedModel Map(string httpResponse)
		{
			var page = new HtmlDocument();
			page.LoadHtml(httpResponse);

			(string firstName, string lastName) = _scraper.ExtractNames(page);
			(int height, int weight) = _scraper.ExtractHeightWeight(page);
			DateTimeOffset dateOfBirth = _scraper.ExtractDateOfBirth(page);
			string college = _scraper.ExtractCollege(page);
			(string esbId, string gsisId) = _scraper.ExtractIds(page);

			return new PlayerAddVersionedModel
			{
				FirstName = firstName,
				LastName = lastName,
				EsbId = esbId,
				GsisId = gsisId,
				Height = height,
				Weight = weight,
				DateOfBirth = dateOfBirth.DateTime,
				College = college
			};
		}
	}
}
