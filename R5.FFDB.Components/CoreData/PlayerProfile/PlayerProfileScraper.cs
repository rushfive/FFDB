using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace R5.FFDB.Components.CoreData.PlayerProfile
{
	// todo: should all be internal
	// todo: integrate to IOC with logging
	public static class PlayerProfileScraper
	{
		public static (string esbId, string gsisId) ExtractIds(HtmlDocument page)
		{
			string[] idCommentLines = page.DocumentNode
				.SelectSingleNode("//comment()[contains(., 'GSIS')]")
				.InnerHtml
				.Split("\n\t");

			// "ESB ID: ADA218591"
			string esbId = idCommentLines
				.Single(l => l.Contains("ESB"))
				.Split(":")[1]
				.Trim();

			// "GSIS ID: 00-0031381"
			string gsisId = idCommentLines
				.Single(l => l.Contains("GSIS"))
				.Split(":")[1]
				.Trim();

			return (esbId, gsisId);
		}

		public static string ExtractPictureUri(HtmlDocument page)
		{
			return page.DocumentNode.SelectNodes("//meta")
				.SingleOrDefault(n => n.Attributes.Contains("property") && n.Attributes["property"].Value == "og:image")
				?.Attributes["content"].Value;
		}

		public static (int height, int weight) ExtractHeightWeight(HtmlDocument page)
		{
			HtmlNodeCollection infoParagraphs = GetInfoParagraphNodes(page);

			// InnerText:
			// "\r\n\t\t\t\t\tHeight: 5-10 &nbsp; \r\n\t\t\t\t\tWeight: 192 &nbsp; \r\n\t\t\t\t\t\r\n\t\t\t\t\t\t\r\n\t\t\t\t\t\t\r\n\t\t\t\t\t\t\tAge: 30\r\n\t\t\t\t\t\t\r\n\t\t\t\t\t\r\n\t\t\t\t"
			HtmlNode heightWeightParagraph = infoParagraphs
				.FirstOrDefault(p => p.InnerText.Contains("Height") && p.InnerText.Contains("Weight"));
			if (heightWeightParagraph == null)
			{
				throw new InvalidOperationException("Failed to scrape height and weight.");
			}

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

			HtmlNode dateOfBirthParagraph = infoParagraphs.FirstOrDefault(p => p.InnerText.Contains("Born:", StringComparison.OrdinalIgnoreCase));
			if (dateOfBirthParagraph == null)
			{
				throw new InvalidOperationException("Failed to scrape date of birth.");
			}

			var spaceSplit = dateOfBirthParagraph.InnerText.Split(" ");
			return DateTimeOffset.Parse(spaceSplit[1]);
		}

		public static string ExtractCollege(HtmlDocument page)
		{
			HtmlNodeCollection infoParagraphs = GetInfoParagraphNodes(page);
			HtmlNode collegeParagraph = infoParagraphs[4];

			var spaceSplit = collegeParagraph.InnerText.Trim().Split(" ");
			return HtmlEntity.DeEntitize(spaceSplit[1]);
		}

		public static (string firstName, string lastName) ExtractNames(HtmlDocument page)
		{
			HtmlNodeCollection infoParagraphs = GetInfoParagraphNodes(page);
			HtmlNode nameParagraph = infoParagraphs[0];
			HtmlNode name = nameParagraph.ChildNodes.Single(n => n.HasClass("player-name"));

			string fullName = name.InnerText;
			string stripped = Regex.Replace(fullName, @"&nbsp;", "").Trim();

			var split = stripped.Split(" ");

			string firstName = split[0];

			string lastName = null;
			if (split.Length > 1)
			{
				lastName = string.Join(" ", split.Skip(1)).Trim();
			}

			return (
				HtmlEntity.DeEntitize(firstName), 
				HtmlEntity.DeEntitize(lastName));
		}

		private static HtmlNodeCollection GetInfoParagraphNodes(HtmlDocument page)
		{
			HtmlNode bio = page.GetElementbyId("player-bio");
			HtmlNode info = bio.ChildNodes.Single(n => n.HasClass("player-info"));
			return info.SelectNodes("p"); ;
		}

	}
}
