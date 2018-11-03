namespace R5.FFDB.Core.Components.FantasyApi
{
	public static class FantasyApiEndpoint
	{
		public static class V2
		{
			public static string WeekStatsUri(int season, int week)
				=> $"http://api.fantasy.nfl.com/v2/players/weekstats?season={season}&week={week}";
		}
	}
}
