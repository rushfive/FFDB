using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1.Models;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1
{
	public interface IRosterScraper
	{
		List<RosterVersioned.Player> ExtractPlayers(HtmlDocument page);
	}

	public class RosterScraper : IRosterScraper
	{
		private IAppLogger _logger { get; }

		public RosterScraper(IAppLogger logger)
		{
			_logger = logger;
		}

		public List<RosterVersioned.Player> ExtractPlayers(HtmlDocument page)
		{
			var result = new List<RosterVersioned.Player>();

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
				Position position = ExtractPosition(r);
				RosterStatus status = ExtractStatus(r);

				result.Add(new RosterVersioned.Player
				{
					NflId = id,
					Number = number,
					Position = position,
					Status = status
				});

				_logger.LogDebug($"Extracted player '{id}'.");
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

		// NOT necessary, we currently get from player profile pages. Keep this
		// in case for future redundancy
		//private (string firstName, string lastName) ExtractName(HtmlNode playerRow)
		//{
		//	try
		//	{
		//		HtmlNodeCollection tdChildNodes = playerRow.SelectNodes("td")[1].ChildNodes;

		//		string fullName = tdChildNodes[1].InnerText;

		//		string[] commaSplit = fullName.Split(",");
		//		if (commaSplit.Length == 1 || commaSplit.Length > 2)
		//		{
		//			return (fullName, null);
		//		}

		//		return (commaSplit[1].Trim(), commaSplit[0].Trim());
		//	}
		//	catch (Exception ex)
		//	{
		//		_logger.LogError(ex, "Failed to extract first and last name from a player row.");
		//		throw;
		//	}
		//}

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
