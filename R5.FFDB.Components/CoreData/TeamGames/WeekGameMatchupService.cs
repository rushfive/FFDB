using Microsoft.Extensions.Logging;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace R5.FFDB.Components.CoreData.TeamGames
{
	public interface IWeekGameMatchupService
	{
		List<WeekGameMatchup> GetForWeek(WeekInfo week);
	}

	public class WeekGameMatchupService : IWeekGameMatchupService
	{
		private ILogger<WeekGameMatchupService> _logger { get; }
		private DataDirectoryPath _dataPath { get; }

		public WeekGameMatchupService(
			ILogger<WeekGameMatchupService> logger,
			DataDirectoryPath dataPath)
		{
			_logger = logger;
			_dataPath = dataPath;
		}

		public List<WeekGameMatchup> GetForWeek(WeekInfo week)
		{
			var result = new List<WeekGameMatchup>();

			var filePath = _dataPath.Static.TeamGameHistoryWeekGames + $"{week.Season}-{week.Week}.xml";

			XElement weekGameXml = XElement.Load(filePath);

			XElement gameNode = weekGameXml.Elements("gms").Single();

			foreach (XElement game in gameNode.Elements("g"))
			{
				var matchup = new WeekGameMatchup
				{
					Season = week.Season,
					Week = week.Week
				};

				matchup.HomeTeamId = TeamDataStore.GetIdFromAbbreviation(game.Attribute("h").Value, includePriorLookup: true);
				matchup.AwayTeamId = TeamDataStore.GetIdFromAbbreviation(game.Attribute("v").Value, includePriorLookup: true);
				matchup.NflGameId = game.Attribute("eid").Value;
				matchup.GsisGameId = game.Attribute("gsis").Value;

				result.Add(matchup);
			}

			return result;
		}
	}
}
