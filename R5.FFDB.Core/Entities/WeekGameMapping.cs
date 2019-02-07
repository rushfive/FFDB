using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Entities
{
	public class WeekGameMapping
	{
		public WeekInfo Week { get; set; }
		public int HomeTeamId { get; set; }
		public int AwayTeamId { get; set; }
		public string NflGameId { get; set; }
		public string GsisGameId { get; set; }
	}
}
