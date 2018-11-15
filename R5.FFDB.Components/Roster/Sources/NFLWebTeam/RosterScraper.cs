using HtmlAgilityPack;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Components.Roster.Sources.NFLWebTeam
{
	public static class RosterScraper
	{
		public static List<(string nflId, int? number, Position position, RosterStatus status)> ExtractPlayers(HtmlDocument page)
		{
			var result = new List<(string, int?, Position, RosterStatus)>();

			HtmlNodeCollection playerRows = page.GetElementbyId("result")
				?.SelectSingleNode("//tbody")
				?.SelectNodes("tr");

			foreach (HtmlNode r in playerRows)
			{
				string id = ExtractNflId(r);
				int? number = ExtractNumber(r);
				Position position = ExtractPosition(r);
				RosterStatus status = ExtractStatus(r);

				result.Add((id, number, position, status));
			}

			return result;
		}

		private static int? ExtractNumber(HtmlNode playerRow)
		{
			//HtmlNode td = playerRow.SelectNodes("td")[0];
			//var childNodes = td.ChildNodes;

			HtmlNodeCollection tdChildNodes = playerRow.SelectNodes("td")[0].ChildNodes;

			if (!tdChildNodes.Any())
			{
				return null;
			}

			string numberText = tdChildNodes.Single().InnerText;

			if (string.IsNullOrWhiteSpace(numberText) || !int.TryParse(numberText, out int number))
			{
				return null;
			}

			return number;

			//

			//string numberText = playerRow.SelectNodes("td")[0]
			//	.ChildNodes
			//	.Single()
			//	.InnerText;



			//if (string.IsNullOrWhiteSpace(numberText) || !int.TryParse(numberText, out int number))
			//{
			//	return null;
			//}

			//return number;
		}

		private static string ExtractNflId(HtmlNode playerRow)
		{
			// "/player/mauricealexander/2550145/profile"
			string profileUri = playerRow.SelectNodes("td")[1]
				.ChildNodes
				.Single(n => n.NodeType == HtmlNodeType.Element)
				.Attributes["href"]
				.Value;

			string[] slashSplit = profileUri.Split("/");

			Func<string, bool> isNumericString = s =>
				!string.IsNullOrWhiteSpace(s) && s.All(char.IsDigit);

			return slashSplit.First(isNumericString);
		}

		private static Position ExtractPosition(HtmlNode playerRow)
		{
			string position = playerRow.SelectNodes("td")[2]
				.ChildNodes
				.Single()
				.InnerText;

			return Enum.Parse<Position>(position);
		}

		private static RosterStatus ExtractStatus(HtmlNode playerRow)
		{
			string status = playerRow.SelectNodes("td")[3]
				.ChildNodes
				.Single()
				.InnerText;

			return Enum.Parse<RosterStatus>(status);
		}
	}
}
