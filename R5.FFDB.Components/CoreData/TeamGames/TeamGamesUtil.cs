//using Newtonsoft.Json.Linq;
//using R5.FFDB.Core.Entities;
//using R5.FFDB.Core.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Xml.Linq;

//namespace R5.FFDB.Components.CoreData.TeamGames
//{
//	[Obsolete]
//	internal static class TeamGamesUtil
//	{
//		[Obsolete("moving to GameInfoCache")]
//		internal static List<string> GetGameIdsForWeek(WeekInfo week, DataDirectoryPath dataPath)
//		{
//			var result = new List<string>();

//			string filePath = null;// dataPath.Static.WeekGames + $"{week.Season}-{week.Week}.xml";

//			XElement weekGameXml = XElement.Load(filePath);

//			XElement gameNode = weekGameXml.Elements("gms").Single();

//			foreach (XElement game in gameNode.Elements("g"))
//			{
//				string gameId = game.Attribute("eid").Value;
//				result.Add(gameId);
//			}

//			return result;
//		}
//		[Obsolete("move to wherever this is happening")]
//		internal static void SetTeamWeekStats(TeamWeekStats stats,
//			JObject statsJson, string gameId, string teamType)
//		{
//			SetPointsScored(stats, statsJson, gameId, teamType);
//			SetTeamStats(stats, statsJson, gameId, teamType);
//		}

//		private static void SetPointsScored(TeamWeekStats stats,
//			JObject statsJson, string gameId, string teamType)
//		{
//			JToken score = statsJson.SelectToken($"{gameId}.{teamType}.score");
//			if (score == null)
//			{
//				throw new InvalidOperationException($"Failed to parse score object for {teamType} team in game '{gameId}'.");
//			}

//			stats.PointsFirstQuarter = (int)score["1"];
//			stats.PointsSecondQuarter = (int)score["2"];
//			stats.PointsThirdQuarter = (int)score["3"];
//			stats.PointsFourthQuarter = (int)score["4"];
//			stats.PointsOverTime = (int)score["5"];
//			stats.PointsTotal = (int)score["T"];
//		}

//		private static void SetTeamStats(TeamWeekStats stats,
//			JObject statsJson, string gameId, string teamType)
//		{
//			JToken teamStats = statsJson.SelectToken($"{gameId}.{teamType}.stats.team");
//			if (teamStats == null)
//			{
//				throw new InvalidOperationException($"Failed to parse team stats object for {teamType} team in game '{gameId}'.");
//			}

//			stats.FirstDowns = (int)teamStats["totfd"];
//			stats.TotalYards = (int)teamStats["totyds"];
//			stats.PassingYards = (int)teamStats["pyds"];
//			stats.RushingYards = (int)teamStats["ryds"];
//			stats.Penalties = (int)teamStats["pen"];
//			stats.PenaltyYards = (int)teamStats["penyds"];
//			stats.Turnovers = (int)teamStats["trnovr"];
//			stats.Punts = (int)teamStats["pt"];
//			stats.PuntYards = (int)teamStats["ptyds"];
//			//stats.PuntYardsAverage = (int)teamStats["ptavg"];

//			string timeOfPosession = (string)teamStats["top"];
//			var split = timeOfPosession.Split(':');
//			stats.TimeOfPossessionSeconds = int.Parse(split[0]) * 60 + int.Parse(split[1]);
//		}
//	}
//}
