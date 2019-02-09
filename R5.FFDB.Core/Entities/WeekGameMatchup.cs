using R5.FFDB.Core.Models;

namespace R5.FFDB.Core.Entities
{
	public class WeekGameMatchup
	{
		public WeekInfo Week { get; set; }
		public int HomeTeamId { get; set; }
		public int AwayTeamId { get; set; }
		public string NflGameId { get; set; }
		public string GsisGameId { get; set; }

		public override string ToString()
		{
			return $"{HomeTeamId} vs {AwayTeamId} ({Week})";
		}
	}
}
