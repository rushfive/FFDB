using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Models
{
	public class Roster
	{
		public int TeamId { get; set; }
		public string TeamAbbreviation { get; set; }
		public List<RosterPlayer> Players { get; set; }

		public override string ToString()
		{
			return $"{TeamAbbreviation} Roster";
		}
	}

	public class RosterPlayer
	{
		public string NflId { get; set; }
		public int? Number { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public Position Position { get; set; }
		public RosterStatus Status { get; set; }
	}
}
