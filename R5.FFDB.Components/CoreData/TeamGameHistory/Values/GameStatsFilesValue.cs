//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json.Linq;
//using R5.FFDB.Components.Resolvers;
//using R5.FFDB.Components.ValueProviders;
//using R5.FFDB.Core.Models;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;

//namespace R5.FFDB.Components.CoreData.TeamGameHistory.Values
//{
//	// Value: List of tuples containing game id, week, and parsed json representing its game stats
//	public class GameStatsFilesValue : ValueProvider<List<(string, WeekInfo, JObject)>>
//	{
//		private ILogger<GameStatsFilesValue> _logger { get; }
//		private DataDirectoryPath _dataPath { get; }
//		private GameWeekMapValue _gameWeekMap { get; }

//		public GameStatsFilesValue(
//			ILogger<GameStatsFilesValue> logger,
//			DataDirectoryPath dataPath,
//			GameWeekMapValue gameWeekMap)
//			: base("Game Stats Files")
//		{
//			_logger = logger;
//			_dataPath = dataPath;
//			_gameWeekMap = gameWeekMap;
//		}

//		protected override List<(string, WeekInfo, JObject)> ResolveValue()
//		{
//			_logger.LogDebug("Parsing every existing game stats json file. This will take a second..");

//			var result = new List<(string gameId, WeekInfo week, JObject json)>();

//			List<string> gameStatFiles = DirectoryFilesResolver.GetFileNames(
//				_dataPath.Static.TeamGameHistoryGameStats, 
//				excludeExtensions: true);

//			Dictionary<string, WeekInfo> gameWeekMap = _gameWeekMap.Get();

//			foreach (string gameId in gameStatFiles)
//			{
//				WeekInfo week = gameWeekMap[gameId];
//				JObject json = JObject.Parse(File.ReadAllText(_dataPath.Static.TeamGameHistoryGameStats + $"{gameId}.json"));

//				result.Add((gameId, week, json));
//			}

//			_logger.LogInformation("Finished parsing game stat json files.");
//			return result.OrderBy(i => i.week).ToList();
//		}
//	}
//}
