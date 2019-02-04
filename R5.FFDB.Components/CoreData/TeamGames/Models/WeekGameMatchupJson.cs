using Newtonsoft.Json;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData.TeamGames.Models
{
	public class WeekGameMatchups
	{
		[JsonProperty("week")]
		public WeekInfo Week { get; set; }

		[JsonProperty("matchups")]
		public List<Matchup> Matchups { get; set; } = new List<Matchup>();


		public class Matchup
		{
			[JsonProperty("homeTeamId")]
			public int HomeTeamId { get; set; }

			[JsonProperty("awayTeamId")]
			public int AwayTeamId { get; set; }

			[JsonProperty("nflGameId")]
			public string NflGameId { get; set; }

			[JsonProperty("gsisGameId")]
			public string GsisGameId { get; set; }

			public static WeekGameMatchup ToCoreEntity(Matchup matchup, WeekInfo week)
			{
				return new WeekGameMatchup
				{
					Season = week.Season,
					Week = week.Week,
					HomeTeamId = matchup.HomeTeamId,
					AwayTeamId = matchup.AwayTeamId,
					NflGameId = matchup.NflGameId,
					GsisGameId = matchup.GsisGameId
				};
			}
		}
	}
}
