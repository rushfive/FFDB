using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace R5.FFDB.Components.CoreData.TeamGames
{
	internal static class GameFilesUtil
	{
		internal static List<string> GetGameIdsForWeek(WeekInfo week, DataDirectoryPath dataPath)
		{
			var result = new List<string>();

			var filePath = dataPath.Static.TeamGameHistoryWeekGames + $"{week.Season}-{week.Week}.xml";

			XElement weekGameXml = XElement.Load(filePath);

			XElement gameNode = weekGameXml.Elements("gms").Single();

			foreach (XElement game in gameNode.Elements("g"))
			{
				string gameId = game.Attribute("eid").Value;
				result.Add(gameId);
			}

			return result;
		}
	}
}
