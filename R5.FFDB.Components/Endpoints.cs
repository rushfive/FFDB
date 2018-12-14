using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components
{
	public static class Endpoints
	{
		public static class Api
		{
			// json
			public static string WeekStats(int season, int week)
			{
				return $"http://api.fantasy.nfl.com/v2/players/weekstats?season={season}&week={week}";
			}

			// xml
			public static string ScoreStripWeekGames(int season, int week)
			{
				return $"http://www.nfl.com/ajax/scorestrip?season={season}&seasonType=REG&week={week}";
			}

			// json
			public static string GameCenterStats(string gameId)
			{
				return $"http://www.nfl.com/liveupdate/game-center/{gameId}/{gameId}_gtd.json";
			}
		}

		public static class Page
		{
			public static string NFLWebPlayerProfile(string nflId)
			{
				return $"http://www.nfl.com/player/{nflId}/{nflId}/profile";
			}
		}
	}
}
