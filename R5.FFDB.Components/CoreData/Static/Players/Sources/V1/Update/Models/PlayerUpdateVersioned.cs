using Newtonsoft.Json;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Update.Models
{
	public class PlayerUpdateVersioned
	{
		// current source: player profile pages

		[JsonProperty("firstName")]
		public string FirstName { get; set; }

		[JsonProperty("lastName")]
		public string LastName { get; set; }

		// current source: roster pages

		[JsonProperty("number")]
		public int? Number { get; set; }

		[JsonProperty("position")]
		public Position? Position { get; set; }

		[JsonProperty("status")]
		public RosterStatus? Status { get; set; }

		public static PlayerUpdate ToCoreEntity(PlayerUpdateVersioned versioned,
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
