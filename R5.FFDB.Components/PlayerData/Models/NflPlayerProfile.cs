using System;

namespace R5.FFDB.Components.PlayerData.Models
{
	public class NflPlayerProfile
	{
		public int Number { get; set; }
		public int Height { get; set; }
		public int Weight { get; set; }
		public DateTimeOffset DateOfBirth { get; set; }
		public string College { get; set; }
	}
}
