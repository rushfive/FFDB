using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace R5.FFDB.Components.CoreData.TeamGameHistory.Values
{
	public class GameWeekMapValue : ValueProvider<Dictionary<string, WeekInfo>>
	{
		private DataDirectoryPath _dataPath { get; }

		public GameWeekMapValue(DataDirectoryPath dataPath)
			: base("Game Week Map")
		{
			_dataPath = dataPath;
		}

		protected override Dictionary<string, WeekInfo> ResolveValue()
		{
			var result = new Dictionary<string, WeekInfo>();

			List<string> weekGameFilePaths = new DirectoryInfo(_dataPath.Static.TeamGameHistoryWeekGames)
				.GetFiles()
				.Select(f => f.ToString())
				.ToList();

			if (!weekGameFilePaths.Any())
			{
				throw new InvalidOperationException("Game week files haven't been fetched yet.");
			}

			foreach (string filePath in weekGameFilePaths)
			{
				XElement weekGameXml = XElement.Load(filePath);

				XElement gameNode = weekGameXml.Elements("gms").Single();

				var week = new WeekInfo(
					int.Parse(gameNode.Attribute("y").Value),
					int.Parse(gameNode.Attribute("w").Value));

				foreach (XElement game in gameNode.Elements("g"))
				{
					string gameId = game.Attribute("eid").Value;
					result[gameId] = week;
				}
			}

			return result;
		}
	}
}
