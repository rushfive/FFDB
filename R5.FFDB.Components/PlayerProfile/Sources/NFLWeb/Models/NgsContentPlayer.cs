using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.PlayerProfile.Sources.NFLWeb.Models
{
	public class NgsContentPlayer
	{
		public string NflId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public Position Position { get; set; }
		public int? TeamId { get; set; } // not active if null
	}
}
