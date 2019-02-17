namespace R5.FFDB.DbProviders.Mongo.Collections
{
	public static class Collection
	{
		internal const string FfdbPrefix = "ffdb.";

		internal const string Player = FfdbPrefix + "player";
		internal const string Team = FfdbPrefix + "team";
		internal const string UpdateLog = FfdbPrefix + "updateLog";
		internal const string WeekMatchup = FfdbPrefix + "weekMatchup";
		internal const string WeekStatsDst = FfdbPrefix + "weekStatsDst";
		internal const string WeekStatsPlayer = FfdbPrefix + "weekStatsPlayer";
		internal const string WeekStatsTeam = FfdbPrefix + "weekStatsTeam";
	}
}
