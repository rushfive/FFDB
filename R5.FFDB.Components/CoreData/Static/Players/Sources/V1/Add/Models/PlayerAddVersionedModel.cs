using Newtonsoft.Json;
using R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Update.Models;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Add.Models
{
	public class PlayerAddVersionedModel
	{
		[JsonProperty("nflId")]
		public string NflId { get; set; }

		[JsonProperty("esbId")]
		public string EsbId { get; set; }

		[JsonProperty("gsisId")]
		public string GsisId { get; set; }

		[JsonProperty("firstName")]
		public string FirstName { get; set; }

		[JsonProperty("lastName")]
		public string LastName { get; set; }

		[JsonProperty("height")]
		public int Height { get; set; }

		[JsonProperty("weight")]
		public int Weight { get; set; }

		[JsonProperty("dateOfBirth")]
		public DateTimeOffset DateOfBirth { get; set; }

		[JsonProperty("college")]
		public string College { get; set; }

		public static PlayerAdd ToCoreEntity(PlayerAddVersionedModel versioned,
			int? number, Position? position, RosterStatus? status)
		{
			return new PlayerAdd
			{
				Id = Guid.NewGuid(),
				NflId = versioned.NflId,
				EsbId = versioned.EsbId,
				GsisId = versioned.EsbId,
				FirstName = versioned.FirstName,
				LastName = versioned.LastName,
				Height = versioned.Height,
				Weight = versioned.Weight,
				DateOfBirth = versioned.DateOfBirth,
				College = versioned.College,
				Number = number,
				Position = position,
				Status = status
			};
		}
	}
}
