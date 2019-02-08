using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Update.Models
{
	public class PlayerUpdateVersionedModel
	{
		// current source: player profile pages
		public string FirstName { get; set; }
		public string LastName { get; set; }

		// current source: roster pages
		public int? Number { get; set; }
		public Position? Position { get; set; }
		public RosterStatus? Status { get; set; }

		public static PlayerUpdate ToCoreEntity(PlayerUpdateVersionedModel versioned,
			int? number, Position? position, RosterStatus? status)
		{
			return new PlayerUpdate
			{
				FirstName = versioned.FirstName,
				LastName = versioned.LastName,
				Number = number,
				Position = position,
				Status = status
			};
		}
	}
}
