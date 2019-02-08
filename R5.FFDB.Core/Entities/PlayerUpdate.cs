using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Entities
{
	public class PlayerUpdate
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int? Number { get; set; }
		public Position? Position { get; set; }
		public RosterStatus? Status { get; set; }

		public override string ToString()
		{
			return $"{FirstName} {LastName}".Trim();
		}
	}
}
