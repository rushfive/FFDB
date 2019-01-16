namespace R5.FFDB.DbProviders.PostgreSql
{
	public static class Table
	{
		public const string Player = "ffdb.player";
		public const string PlayerTeamMap = "ffdb.player_team_map";
		public const string TeamGameStats = "ffdb.team_game_stats";
		public const string Team = "ffdb.team";
		public const string UpdateLog = "ffdb.update_log";
		public const string WeekGameMatchup = "ffdb.week_game_matchup";

		public static class WeekStats
		{
			public const string DST = "ffdb.week_stats_dst";
			public const string IDP = "ffdb.week_stats_idp";
			public const string Kick = "ffdb.week_stats_kick";
			public const string Misc = "ffdb.week_stats_misc";
			public const string Pass = "ffdb.week_stats_pass";
			public const string Receive = "ffdb.week_stats_receive";
			public const string Rush = "ffdb.week_stats_rush";
			public const string Return = "ffdb.week_stats_return";
		}
	}
}
