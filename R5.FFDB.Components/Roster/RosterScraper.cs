using HtmlAgilityPack;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Components.Roster
{
	public static class RosterScraper
	{
		public static List<(string nflId, Position position, RosterStatus status)> ExtractPlayers(HtmlDocument page)
		{
			var result = new List<(string, Position, RosterStatus)>();

			HtmlNodeCollection rows = page.GetElementbyId("result")
				?.SelectSingleNode("//tbody")
				?.SelectNodes("tr");

			foreach (HtmlNode r in rows)
			{
				string id = ExtractNflId(r);
				Position position = ExtractPosition(r);
				RosterStatus status = ExtractStatus(r);

				result.Add((id, position, status));
			}

			return result;
		}

		private static string ExtractNflId(HtmlNode row)
		{
			// "/player/mauricealexander/2550145/profile"
			string profileUri = row.SelectNodes("td")[1]
				.ChildNodes
				.Single(n => n.NodeType == HtmlNodeType.Element)
				.Attributes["href"]
				.Value;

			string[] slashSplit = profileUri.Split("/");

			Func<string, bool> isNumericString = s =>
				!string.IsNullOrWhiteSpace(s) && s.All(char.IsDigit);

			return slashSplit.First(isNumericString);
		}

		private static Position ExtractPosition(HtmlNode row)
		{
			string position = row.SelectNodes("td")[2]
				.ChildNodes
				.Single()
				.InnerText;

			return Enum.Parse<Position>(position);
		}

		private static RosterStatus ExtractStatus(HtmlNode row)
		{
			string status = row.SelectNodes("td")[3]
				.ChildNodes
				.Single()
				.InnerText;

			return Enum.Parse<RosterStatus>(status);
		}
	}
}
