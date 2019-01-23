using System;

namespace R5.FFDB.Core.Entities
{
	public class Player
	{
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

		public override string ToString()
		{
			return $"{FirstName} {LastName}";
		}
	}
}
