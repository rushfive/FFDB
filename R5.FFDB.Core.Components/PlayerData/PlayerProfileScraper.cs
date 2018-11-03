using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Core.Components.PlayerData
{
	// todo: should all be internal
	public static class PlayerProfileScraper
	{

		public static int ExtractPlayerNumber(HtmlDocument page)
		{
			HtmlNodeCollection infoParagraphs = GetInfoParagraphNodes(page);
			HtmlNode playerNumberParagraph = infoParagraphs[0].ChildNodes.Single(n => n.HasClass("player-number"));

			// InnerText:
			// #89 WR
			string[] textSplit = playerNumberParagraph.InnerText.Split(" ");
			string numberToken = textSplit.Single(t => t.StartsWith("#"));

			return int.Parse(numberToken.Substring(1));
		}

		public static (int height, int weight) ExtractHeightWeight(HtmlDocument page)
		{
			HtmlNodeCollection infoParagraphs = GetInfoParagraphNodes(page);
			HtmlNode heightWeightParagraph = infoParagraphs[2];

			// InnerText:
			// "\r\n\t\t\t\t\tHeight: 5-10 &nbsp; \r\n\t\t\t\t\tWeight: 192 &nbsp; \r\n\t\t\t\t\t\r\n\t\t\t\t\t\t\r\n\t\t\t\t\t\t\r\n\t\t\t\t\t\t\tAge: 30\r\n\t\t\t\t\t\t\r\n\t\t\t\t\t\r\n\t\t\t\t"
			string[] colonSplit = heightWeightParagraph.InnerText.Split(":");

			int height = extractHeight(colonSplit[1]);
			int weight = extractWeight(colonSplit[2]);

			return (height, weight);

			int extractHeight(string segmentContainingHeight)
			{
				var spaceSplit = segmentContainingHeight.Trim().Split(" ");
				var dashSplit = spaceSplit[0].Split("-"); // "5-10"
				
				return int.Parse(dashSplit[0]) * 12 + int.Parse(dashSplit[1]);
			}

			int extractWeight(string segmentContainingWeight)
			{
				var spaceSplit = segmentContainingWeight.Trim().Split(" ");
				return int.Parse(spaceSplit[0]);
			}
		}

		public static DateTimeOffset ExtractDateOfBirth(HtmlDocument page)
		{
			HtmlNodeCollection infoParagraphs = GetInfoParagraphNodes(page);
			HtmlNode dateOfBirthParagraph = infoParagraphs[3];
			
			var spaceSplit = dateOfBirthParagraph.InnerText.Split(" ");
			return DateTimeOffset.Parse(spaceSplit[1]);
		}

		public static string ExtractCollege(HtmlDocument page)
		{
			HtmlNodeCollection infoParagraphs = GetInfoParagraphNodes(page);
			HtmlNode collegeParagraph = infoParagraphs[4];

			var spaceSplit = collegeParagraph.InnerText.Trim().Split(" ");
			return spaceSplit[1];
		}

		private static HtmlNodeCollection GetInfoParagraphNodes(HtmlDocument page)
		{
			HtmlNode bio = page.GetElementbyId("player-bio");
			HtmlNode info = bio.ChildNodes.Single(n => n.HasClass("player-info"));
			return info.SelectNodes("p"); ;
		}

	}
}
