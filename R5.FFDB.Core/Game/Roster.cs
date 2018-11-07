using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Game
{
	public class Roster
	{
		public int TeamId { get; set; }
		public List<RosterPlayer> Players { get; set; }
	}

	public class RosterPlayer
	{
		public string NflId { get; set; }
		public Position Position { get; set; }
		public RosterStatus Status { get; set; }
	}
}
