namespace R5.FFDB.Core.Entities
{
	public class WeekGameMatchup
	{
		public int Season { get; set; }
		public int Week { get; set; }
		public int HomeTeamId { get; set; }
		public int AwayTeamId { get; set; }
		public string NflGameId { get; set; }
		public string GsisGameId { get; set; }
	}
}
