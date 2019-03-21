using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Entities
{
	public class Player
	{
		// Static properties (with the rare possible exception of name changes)
		public Guid Id { get; set; }
		public string NflId { get; set; }
		public string EsbId { get; set; }
		public string GsisId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }

		public override string ToString()
		{
			string name = $"{FirstName} {LastName}".Trim();
			return $"{NflId} ({name})";
		}
	}
}
