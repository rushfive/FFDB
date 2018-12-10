using HtmlAgilityPack;
using R5.FFDB.Components.Stores;
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

			// the rows we want dont contain a class in the first <td>
			IEnumerable<HtmlNode> careerStatTableRows = page.DocumentNode
				.SelectSingleNode("//*[@id='player-stats-wrapper']")
				.SelectSingleNode("./table[2]")
				.SelectNodes("./tr")
				.Where(r => !r.SelectNodes("td")[0].Attributes.Contains("class"));

			foreach (HtmlNode row in careerStatTableRows)
			{
				int season = int.Parse(row.SelectNodes("td")[0].InnerText.Trim());
				result.Add(season);
			}

			return result;
		}

		// players that are on IR or suspended dont have rows for the week, even if 
		// they're still on the team.
		public static Dictionary<int, int> ExtractHistoryForSeason(HtmlDocument page)
		{
			var result = new Dictionary<int, int>();

			HtmlNode gameLogsTableBody = page.DocumentNode
				.SelectSingleNode("//*[@id='player_profile_tabs_0']")
				.SelectSingleNode("./table[2]")
				.ChildNodes
				.Single(n => n.Name == "tbody");

			// the actual game log rows contains exactly 45 child nodes.
			List<HtmlNode> gameLogRows = gameLogsTableBody.ChildNodes
				.Where(n =>
				{
					HtmlNodeCollection childNodes = n.ChildNodes;

					// border <tr>s only contain a single child
					if (n.Name != "tr" || childNodes.Count == 1)
					{
						return false;
					}

					List<HtmlNode> tdNodes = childNodes.Where(cn => cn.Name == "td").ToList();

					if (string.Equals(tdNodes[1].InnerText.Trim(), "Bye", StringComparison.OrdinalIgnoreCase))
					{
						return false;
					}

					string weekText = tdNodes.First().InnerText.Trim();
					return !string.IsNullOrWhiteSpace(weekText) && int.TryParse(weekText, out _);
				})
				.ToList();

			foreach(HtmlNode row in gameLogRows)
			{
				HtmlNodeCollection tds = row.SelectNodes("td");
				
				int week = ExtractWeek(tds[0]);
				string opponentAbbreviation = ExtractOpponentAbbreviation(tds[2]);
				int currentTeamId = ExtractCurrentTeamId(tds[3], opponentAbbreviation);

				result[week] = currentTeamId;
			}

			return result;
		}

		private static int ExtractWeek(HtmlNode td)
		{
			return int.Parse(td.InnerText.Trim());
		}

		private static string ExtractOpponentAbbreviation(HtmlNode td)
		{
			// /teams/profile?team=GB
			string anchorValue = td.ChildNodes
				.Last(n => n.Name == "a")
				.Attributes
				.Single(a => a.Name == "href")
				.Value;

			return anchorValue.Split("=")[1];
		}

		private static int ExtractCurrentTeamId(HtmlNode td, string opponentAbbreviation)
		{
			// /gamecenter/2017091010/2017/REG1/seahawks@packers
			string anchorValue = td.ChildNodes
				.Single(n => n.Name == "a")
				.Attributes
				.Single(a => a.Name == "href")
				.Value;

			string[] playingTeams = anchorValue
				.Split("/")
				.Last()
				.Split("@");

			var opponentShortName = TeamDataStore.GetShortNameFromAbbreviation(opponentAbbreviation);

			string currentTeam = playingTeams[0] == opponentShortName ? playingTeams[1] : playingTeams[0];

			return TeamDataStore.GetIdFromShortName(currentTeam);
		}
	}
}
