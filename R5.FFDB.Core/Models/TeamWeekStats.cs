using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Models
{
	public class TeamWeekStats
	{
		public int TeamId { get; }
		public WeekInfo Week { get; }

		// points
		public int PointsFirstQuarter { get; private set; }
		public int PointsSecondQuarter { get; private set; }
		public int PointsThirdQuarter { get; private set; }
		public int PointsFourthQuarter { get; private set; }
		public int PointsOverTime { get; private set; }
		public int PointsTotal { get; private set; }

		// stats
		public int FirstDowns { get; private set; }
		public int TotalYards { get; private set; }
		public int PassingYards { get; private set; }
		public int RushingYards { get; private set; }
		public int Penalties { get; private set; }
		public int PenaltyYards { get; private set; }
		public int Turnovers { get; private set; }
		public int Punts { get; private set; }
		public int PuntYards { get; private set; }
		public int PuntYardsAverage { get; private set; }
		public int TimeOfPossessionSeconds { get; private set; }

		public TeamWeekStats(int teamId, WeekInfo week)
		{
			TeamId = teamId;
			Week = week;
		}

		public void SetPointsScored(JObject gameStats,
			string gameId, string teamType)
		{
			JToken score = gameStats.SelectToken($"{gameId}.{teamType}.score");
			if (score == null)
			{
				throw new InvalidOperationException($"Failed to parse score object for {teamType} team in game '{gameId}'.");
			}

			PointsFirstQuarter = (int)score["1"];
			PointsSecondQuarter = (int)score["2"];
			PointsThirdQuarter = (int)score["3"];
			PointsFourthQuarter = (int)score["4"];
			PointsOverTime = (int)score["5"];
			PointsTotal = (int)score["T"];
		}

		public void SetTeamStats(JObject gameStats,
			string gameId, string teamType)
		{
			JToken teamStats = gameStats.SelectToken($"{gameId}.{teamType}.stats.team");
			if (teamStats == null)
			{
				throw new InvalidOperationException($"Failed to parse team stats object for {teamType} team in game '{gameId}'.");
			}

			FirstDowns = (int)teamStats["totfd"];
			TotalYards = (int)teamStats["totyds"];
			PassingYards = (int)teamStats["pyds"];
			RushingYards = (int)teamStats["ryds"];
			Penalties = (int)teamStats["pen"];
			PenaltyYards = (int)teamStats["penyds"];
			Turnovers = (int)teamStats["trnovr"];
			Punts = (int)teamStats["pt"];
			PuntYards = (int)teamStats["ptyds"];
			PuntYardsAverage = (int)teamStats["ptavg"];

			string timeOfPosession = (string)teamStats["top"];
			var split = timeOfPosession.Split(':');
			TimeOfPossessionSeconds = int.Parse(split[0]) * 60 + int.Parse(split[1]);
		}
	}
}
