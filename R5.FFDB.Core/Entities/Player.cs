using R5.FFDB.Core.Models;
using System;

namespace R5.FFDB.Core.Entities
{
	// Data to compose this model currently requires 2 different sources:
	// NFL player profile page for static properties, and
	// NFL team roster pages for the dynamic ones
	public class Player
	{
		// Static properties (with the rare possible exception of name changes)
		public Guid Id { get; set; }
		public string NflId { get; set; }
		public string EsbId { get; set; }
		public string GsisId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int Height { get; set; }
		public int Weight { get; set; }
		public DateTimeOffset DateOfBirth { get; set; }
		public string College { get; set; }

		// Dynamic and more likely to change.
		public int? Number { get; set; }
		public Position? Position { get; set; }
		public RosterStatus? Status { get; set; }

		public override string ToString()
		{
			string name = $"{FirstName} {LastName}".Trim();
			return $"{NflId} ({name})";
		}
	}
}
