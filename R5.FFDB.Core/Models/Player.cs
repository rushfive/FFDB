using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Core.Models
{
	public class Player
	{
		public string Id => $"{FirstName[0]}{LastName[0]}{NflId}".ToUpper();
		public bool IsActive => TeamId.HasValue;

		public string NflId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public Position Position { get; set; }
		public int? TeamId { get; set; }
		public int Number { get; set; }
		public int Height { get; set; }
		public int Weight { get; set; }
		public DateTimeOffset DateOfBirth { get; set; }
		public string College { get; set; }

		// some NFL legacy id, stil needed for some stuff
		public string EsbId { get; set; }
	}
}
