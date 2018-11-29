using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.PlayerProfile.Sources.NFLWeb.Models
{
	public class NflPlayerProfile
	{
		public string EsbId { get; set; }
		public string GsisId { get; set; }
		public string PictureUri { get; set; }
		//public int Number { get; set; }
		public int Height { get; set; }
		public int Weight { get; set; }
		public DateTimeOffset DateOfBirth { get; set; }
		public string College { get; set; }
	}
}
