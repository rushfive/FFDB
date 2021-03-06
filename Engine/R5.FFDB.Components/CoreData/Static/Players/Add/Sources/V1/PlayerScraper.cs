﻿using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using R5.FFDB.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace R5.FFDB.Components.CoreData.Static.Players.Add.Sources.V1
{
	public interface IPlayerScraper
	{
		(string esbId, string gsisId) ExtractIds(HtmlDocument page);
		(int height, int weight) ExtractHeightWeight(HtmlDocument page);
		DateTimeOffset ExtractDateOfBirth(HtmlDocument page);
		string ExtractCollege(HtmlDocument page);
		(string firstName, string lastName) ExtractNames(HtmlDocument page);
	}

	public class PlayerScraper : IPlayerScraper
	{
		private IAppLogger _logger { get; }

		public PlayerScraper(IAppLogger logger)
		{
			_logger = logger;
		}

		public (string esbId, string gsisId) ExtractIds(HtmlDocument page)
		{
			string[] idCommentLines = null;
			try
			{
				idCommentLines = page.DocumentNode
					.SelectSingleNode("//comment()[contains(., 'GSIS')]")
					.InnerHtml
					.Split("\n\t");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to find HTML comments on page containing player's ESB and GSIS id.");
			}

			// "ESB ID: ADA218591"
			string esbId = null;
			try
			{
				esbId = idCommentLines
					.Single(l => l.Contains("ESB"))
					.Split(":")[1]
					.Trim();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to find ESB id.");
			}

			// "GSIS ID: 00-0031381"
			string gsisId = null;
			try
			{
				gsisId = idCommentLines
					.Single(l => l.Contains("GSIS"))
					.Split(":")[1]
					.Trim();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to find GSIS id.");
			}

			return (esbId, gsisId);
		}

		public (int height, int weight) ExtractHeightWeight(HtmlDocument page)
		{
			HtmlNodeCollection infoParagraphs = GetInfoParagraphNodes(page);

			HtmlNode heightWeightParagraph = null;
			try
			{
				heightWeightParagraph = infoParagraphs
					.FirstOrDefault(p => p.InnerText.Contains("Height") && p.InnerText.Contains("Weight"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to find paragraph containing player's height and weight.");
			}

			string[] colonSplit = heightWeightParagraph.InnerText.Split(":");

			int height = -1;
			try
			{
				var spaceSplit = colonSplit[1].Trim().Split(" ");
				var dashSplit = spaceSplit[0].Split("-"); // "5-10"

				height = int.Parse(dashSplit[0]) * 12 + int.Parse(dashSplit[1]);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to find player's height.");
			}

			int weight = -1;
			try
			{
				var spaceSplit = colonSplit[2].Trim().Split(" ");
				weight = int.Parse(spaceSplit[0]);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to find player's weight.");
			}

			return (height, weight);
		}

		public DateTimeOffset ExtractDateOfBirth(HtmlDocument page)
		{
			HtmlNodeCollection infoParagraphs = GetInfoParagraphNodes(page);

			HtmlNode dateOfBirthParagraph = null;
			try
			{
				dateOfBirthParagraph = infoParagraphs
					.First(p => p.InnerText.Contains("Born:", StringComparison.OrdinalIgnoreCase));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to find paragraph containing player's date of birth.");
			}

			try
			{
				var spaceSplit = dateOfBirthParagraph.InnerText.Split(" ");
				return DateTimeOffset.Parse(spaceSplit[1]);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to parse player's date of birth.");
				return default;
			}
		}

		public string ExtractCollege(HtmlDocument page)
		{
			HtmlNodeCollection infoParagraphs = GetInfoParagraphNodes(page);

			HtmlNode collegeParagraph = null;
			try
			{
				collegeParagraph = infoParagraphs[4];
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to find paragraph containing player's college.");
			}

			try
			{
				var spaceSplit = collegeParagraph.InnerText.Trim().Split(" ");

				return HtmlEntity.DeEntitize(spaceSplit[1]);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to parser player's college.");
				return null;
			}
		}

		public (string firstName, string lastName) ExtractNames(HtmlDocument page)
		{
			HtmlNodeCollection infoParagraphs = GetInfoParagraphNodes(page);

			HtmlNode name = null;
			try
			{
				HtmlNode nameParagraph = infoParagraphs[0];
				name = nameParagraph.ChildNodes.Single(n => n.HasClass("player-name"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to find paragraph containing player's first and last name.");
			}

			string fullName = name.InnerText;
			string stripped = Regex.Replace(fullName, @"&nbsp;", "").Trim();

			var split = stripped.Split(" ");

			string firstName = null;
			try
			{
				firstName = split[0];
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to parse player's first name.");
			}

			string lastName = null;
			if (split.Length > 1)
			{
				try
				{
					lastName = string.Join(" ", split.Skip(1)).Trim();
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to find parser player's last name.");
				}
			}

			return (
				HtmlEntity.DeEntitize(firstName),
				HtmlEntity.DeEntitize(lastName));
		}

		private HtmlNodeCollection GetInfoParagraphNodes(HtmlDocument page)
		{
			try
			{
				HtmlNode bio = page.GetElementbyId("player-bio");
				HtmlNode info = bio.ChildNodes.Single(n => n.HasClass("player-info"));
				return info.SelectNodes("p"); ;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to find player info paragraph nodes.");
				throw;
			}
		}
	}
}
