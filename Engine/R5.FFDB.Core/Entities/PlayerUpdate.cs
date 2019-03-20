using R5.FFDB.Core.Models;

namespace R5.FFDB.Core.Entities
{
	public class PlayerUpdate
	{
		public int? Number { get; set; }
		public Position? Position { get; set; }
		public RosterStatus? Status { get; set; }
	}
}
