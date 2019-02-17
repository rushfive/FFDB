﻿using HtmlAgilityPack;
using R5.FFDB.Components.CoreData.Static.Players.Add.Sources.V1.Models;
using System;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.Players.Add.Sources.V1.Mappers
{
	public interface IToVersionedMapper : IAsyncMapper<string, PlayerAddVersioned, string> { }

	public class ToVersionedMapper : IToVersionedMapper
	{
		private IPlayerScraper _scraper { get; }

		public ToVersionedMapper(IPlayerScraper scraper)
		{
			_scraper = scraper;
		}

		public Task<PlayerAddVersioned> MapAsync(string httpResponse, string nflId)
		{
			var page = new HtmlDocument();
			page.LoadHtml(httpResponse);

			(string firstName, string lastName) = _scraper.ExtractNames(page);
			(int height, int weight) = _scraper.ExtractHeightWeight(page);
			DateTimeOffset dateOfBirth = _scraper.ExtractDateOfBirth(page);
			string college = _scraper.ExtractCollege(page);
			(string esbId, string gsisId) = _scraper.ExtractIds(page);

			return Task.FromResult(new PlayerAddVersioned
			{
				FirstName = firstName,
				LastName = lastName,
				NflId = nflId,
				EsbId = esbId,
				GsisId = gsisId,
				Height = height,
				Weight = weight,
				DateOfBirth = dateOfBirth.DateTime,
				College = college
			});
		}
	}
}