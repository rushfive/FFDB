using R5.FFDB.Core.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Components.PlayerData.Models
{
	public class NgsContentPlayer
	{
		public string NflId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public PositionType Position { get; set; }
		public int? TeamId { get; set; } // not active if null
	}
}
