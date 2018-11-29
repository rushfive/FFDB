using R5.FFDB.Core.Models;

namespace R5.FFDB.Components.Roster.Sources.NFLWebTeam.Models
{
	public class NFLWebRosterPlayer
	{
		public string Id { get; set; }
		public int? Number { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public Position Position { get; set; }
		public RosterStatus Status { get; set; }

		public static RosterPlayer ToCoreEntity(NFLWebRosterPlayer model)
		{
			return new RosterPlayer
			{
				NflId = model.Id,
				Number = model.Number,
				FirstName = model.FirstName,
				LastName = model.LastName,
				Position = model.Position,
				Status = model.Status
			};
		}
	}
}
