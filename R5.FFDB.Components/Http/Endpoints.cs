using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.Http
{
	public static class Endpoints
	{
		public static class Api
		{
			// json
			public static string WeekStats(WeekInfo week)
			{
				return $"http://api.fantasy.nfl.com/v2/players/weekstats?season={week.Season}&week={week.Week}";
			}

			// xml
			public static string ScoreStripWeekGames(WeekInfo week)
			{
				return $"http://www.nfl.com/ajax/scorestrip?season={week.Season}&seasonType=REG&week={week.Week}";
			}

			// json
			public static string GameCenterStats(string gameId)
			{
				return $"http://www.nfl.com/liveupdate/game-center/{gameId}/{gameId}_gtd.json";
			}
		}

		public static class Page
		{
			public static string PlayerProfile(string nflId)
			{
				return $"http://www.nfl.com/player/{nflId}/{nflId}/profile";
			}

			public static string TeamRoster(Team team)
			{
				return $"http://www.nfl.com/teams/{team.ShortName}/roster?team={team.Abbreviation}";
			}

			public static string ReceiverTargets(string gameId)
			{
				string year = gameId.Substring(0, 4);
				return $"https://www.nfl.com/gamecenter/{gameId}/{year}/{gameId}/{gameId}";
			}
		}
	}
}
