using HtmlAgilityPack;
using R5.FFDB.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Core.Components.Scrapers
{
	public class RosterScraper
	{
		public void Scrape(HtmlDocument page)
		{
			try
			{
				ScrapeInternal(page);
			}
			catch (Exception ex)
			{
				// todo
			}
		}

		private void ScrapeInternal(HtmlDocument page)
		{
			HtmlNodeCollection rosterTableRows = page.GetElementbyId("result")
				.SelectSingleNode("//tbody")
				.SelectNodes("tr");

			foreach (HtmlNode r in rosterTableRows)
			{
				
			}
		}

		private static int ExtractPlayerNumber(HtmlNode playerRow)
		{
			HtmlNode td = playerRow.SelectNodes("td")[0];

			// TODO

			return default(int);
		}

		private static (int nflId, string firstName, string lastName) ExtractPlayerIdAndNames(HtmlNode playerRow)
		{
			HtmlNode td = playerRow.SelectNodes("td")[1];

			var anchor = td.ChildNodes.Single(n => n.NodeType == HtmlNodeType.Element);

			string profileUri = anchor.Attributes["href"].Value;
			string name = anchor.InnerText;

			return (0, null, null);
		}

		private static Position ExtractPlayerPosition(HtmlNode playerRow)
		{
			HtmlNode td = playerRow.SelectNodes("td")[2];

			// TODO

			return default(int);
		}

		private static int ExtractPlayerHeight(HtmlNode playerRow)
		{
			HtmlNode td = playerRow.SelectNodes("td")[4];

			// TODO

			return default(int);
		}

		private static int ExtractPlayerWeight(HtmlNode playerRow)
		{
			HtmlNode td = playerRow.SelectNodes("td")[5];

			// TODO

			return default(int);
		}

		private static DateTime ExtractPlayerDateOfBirth(HtmlNode playerRow)
		{
			HtmlNode td = playerRow.SelectNodes("td")[6];

			// TODO

			return default(DateTime);
		}

		private static string ExtractPlayerCollege(HtmlNode playerRow)
		{
			HtmlNode td = playerRow.SelectNodes("td")[8];

			// TODO

			return default(string);
		}
	}
}
