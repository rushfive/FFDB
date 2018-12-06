using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.PlayerTeamHistory.Sources.NFLWeb
{
	public static class PlayerTeamHistoryScraper
	{
		public static List<int> ExtractSeasonsPlayed(HtmlDocument page)
		{
			// https://hexfox.com/p/having-trouble-extracting-the-tbody-element-during-my-web-scrape/
			// DONT select tbody, it doesnt exist in the HTML (browsers just inject it)
			// insteaad, get all TRs that dont have the classes:
			// 'player-table-header' and 'player-table-key'
			// the table has 3 rows in thead


			HtmlNode test = page.DocumentNode
				.SelectSingleNode("//*[@id='player-stats-wrapper']")
				.SelectSingleNode("./table[2]")
				//
				.SelectSingleNode("./tbody");




			var end = "test";

			//var testSecondTable = page.DocumentNode
			//	.SelectSingleNode("//*[@id='player-stats-wrapper']//table[2]");

			//var secondBODY = testSecondTable.SelectSingleNode(".//tbody");

			//var secondBodyROWS = secondBODY.SelectNodes(".//tr");


			//var testSecondTable2 = page.DocumentNode
			//	.SelectNodes("//*[@id='player-stats-wrapper']//table[2]//tbody//tr");
			////

			//HtmlNode wrapper = page.GetElementbyId("player-stats-wrapper");

			//HtmlNode secondTable = wrapper
			//	.SelectSingleNode("//table[2]");

			//HtmlNodeCollection bodyRows = secondTable
			//	.SelectNodes("//tbody//tr");

			//// career stats table rows
			//var careerStatsRows = page.GetElementbyId("player-stats-wrapper")
			//	.SelectSingleNode("//table[2]//tbody");
			//	//.SelectSingleNode("//table[2]/tbody")
			//	//.SelectSingleNode("//tbody")
			//	//.SelectNodes("tr");

			//var tbody = page.GetElementbyId("player-stats-wrapper")
			//	.SelectSingleNode("//table[2]/tbody");

			//var trows = page.GetElementbyId("player-stats-wrapper")
			//	.SelectNodes("//table[2]/tbody/tr");


			//foreach (HtmlNode r in careerStatsRows)
			//{
			//	HtmlNodeCollection tdNodes = r.SelectNodes("td");



			//	var t = "test";
			//}

			return null;
		}

		public static Dictionary<int, int> ExtractHistoryForSeason(int season, HtmlDocument page)
		{
			throw new NotImplementedException();
		}
	}
}
