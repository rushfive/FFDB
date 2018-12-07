using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Components.PlayerTeamHistory.Sources.NFLWeb
{
	public static class PlayerTeamHistoryScraper
	{
		public static List<int> ExtractSeasonsPlayed(HtmlDocument page)
		{
			var result = new List<int>();

			HtmlNodeCollection careerStatTableRows = page.DocumentNode
				.SelectSingleNode("//*[@id='player-stats-wrapper']")
				.SelectSingleNode("./table[2]")
				.SelectNodes("./tr");

			foreach(HtmlNode row in careerStatTableRows)
			{
				HtmlNodeCollection tds = row.SelectNodes("td");

				// the rows we want dont contain a class in the first <td>,
				// while the ignored row's first <td> contain either
				// 'border-td' or 'player-totals' as its class.
				if (tds[0].Attributes.Contains("class"))
				{
					continue;
				}

				int season = int.Parse(tds[0].InnerText.Trim());
				result.Add(season);
			}

			return result;
		}

		public static Dictionary<int, int> ExtractHistoryForSeason(HtmlDocument page)
		{
			IEnumerable<HtmlNode> gameLogTableRows = page.DocumentNode
				.SelectSingleNode("//*[@id='player-stats-wrapper']")
				.SelectSingleNode("./table[2]")
				.SelectNodes("./tr")
				.Where(r => !r.SelectNodes("td")[0].Attributes.Contains("class"));

			//var gameRows = gameLogTableRows.Where(r => !r.SelectNodes("td")[0].Attributes.Contains("class"))

			foreach(HtmlNode row in gameLogTableRows)
			{
				HtmlNodeCollection tds = row.SelectNodes("td");

				if (tds[0].Attributes.Contains("class"))
				{
					continue;
				}

			}

			throw new NotImplementedException();
		}
	}
}
