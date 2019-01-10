using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Components.CoreData.Roster
{
	public interface IRosterScraper
	{
		List<RosterPlayer> ExtractPlayers(HtmlDocument page);
	}

	public class RosterScraper : IRosterScraper
	{
		private ILogger<RosterScraper> _logger { get; }

		public RosterScraper(ILogger<RosterScraper> logger)
		{
			_logger = logger;
		}

		public List<RosterPlayer> ExtractPlayers(HtmlDocument page)
		{
			var result = new List<RosterPlayer>();

			HtmlNodeCollection playerRows = null;
			try
			{
				playerRows = page.GetElementbyId("result")
					?.SelectSingleNode("//tbody")
					?.SelectNodes("tr");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to find players table rows.");
				throw;
			}

			_logger.LogDebug($"Found {playerRows.Count} player rows to scrape.");

			foreach (HtmlNode r in playerRows)
			{
				string id = ExtractNflId(r);
				int? number = ExtractNumber(r);
				(string firstName, string lastName) = ExtractName(r);
				Position position = ExtractPosition(r);
				RosterStatus status = ExtractStatus(r);

				result.Add(new RosterPlayer
				{
					NflId = id,
					Number = number,
					FirstName = firstName,
					LastName = lastName,
					Position = position,
					Status = status
				});

				_logger.LogTrace($"Extracted player '{id}' ({firstName} {lastName}).");
			}

			return result;
		}

		private string ExtractNflId(HtmlNode playerRow)
		{
			try
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
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to extract NFL Id from a player row.");
				throw;
			}
		}

		private int? ExtractNumber(HtmlNode playerRow)
		{
			try
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
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to extract player's number from a player row.");
				throw;
			}
		}

		private (string firstName, string lastName) ExtractName(HtmlNode playerRow)
		{
			try
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
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to extract first and last name from a player row.");
				throw;
			}
		}

		private Position ExtractPosition(HtmlNode playerRow)
		{
			try
			{
				string position = playerRow.SelectNodes("td")[2]
				.ChildNodes
				.Single()
				.InnerText;

				return Enum.Parse<Position>(position);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to extract position from a player row.");
				throw;
			}
		}

		private RosterStatus ExtractStatus(HtmlNode playerRow)
		{
			try
			{
				string status = playerRow.SelectNodes("td")[3]
				.ChildNodes
				.Single()
				.InnerText;

				return Enum.Parse<RosterStatus>(status);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to extract roster status from a player row.");
				throw;
			}
		}
	}
}
