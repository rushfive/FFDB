using HtmlAgilityPack;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Components.CoreData.Roster
{
	public static class RosterScraper
	{
		public static List<RosterPlayer> ExtractPlayers(HtmlDocument page)
		{
			var result = new List<RosterPlayer>();

			HtmlNodeCollection playerRows = page.GetElementbyId("result")
				?.SelectSingleNode("//tbody")
				?.SelectNodes("tr");

			foreach (HtmlNode r in playerRows)
			{
				string id = ExtractNflId(r);
				int? number = ExtractNumber(r);
				(string firstName, string lastName) = ExtractName(r);
				Position position = ExtractPosition(r);
				RosterPlayer.RosterStatus status = ExtractStatus(r);

				result.Add(new RosterPlayer
				{
					NflId = id,
					Number = number,
					FirstName = firstName,
					LastName = lastName,
					Position = position,
					Status = status
				});
			}

			return result;
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

		private static int? ExtractNumber(HtmlNode playerRow)
		{
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
		}

		private static (string firstName, string lastName) ExtractName(HtmlNode playerRow)
		{
			HtmlNodeCollection tdChildNodes = playerRow.SelectNodes("td")[1].ChildNodes;

			string fullName = tdChildNodes[1].InnerText;

			string[] commaSplit = fullName.Split(",");
			if (commaSplit.Length == 1 || commaSplit.Length > 2)
			{
				return (fullName, null);
			}

			return (commaSplit[1].Trim(), commaSplit[0].Trim());
		}

		private static Position ExtractPosition(HtmlNode playerRow)
		{
			string position = playerRow.SelectNodes("td")[2]
				.ChildNodes
				.Single()
				.InnerText;

			return Enum.Parse<Position>(position);
		}

		private static RosterPlayer.RosterStatus ExtractStatus(HtmlNode playerRow)
		{
			string status = playerRow.SelectNodes("td")[3]
				.ChildNodes
				.Single()
				.InnerText;

			return Enum.Parse<RosterPlayer.RosterStatus>(status);
		}
	}
}
