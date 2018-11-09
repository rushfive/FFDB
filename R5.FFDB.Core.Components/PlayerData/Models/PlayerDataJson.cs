using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using R5.FFDB.Core.Models;
using System;

namespace R5.FFDB.Core.Components.PlayerData.Models
{
	// static data retrieved from api and scraping
	public class PlayerDataJson
	{
		[JsonProperty("nflId")]
		public string NflId { get; set; }

		[JsonProperty("firstName")]
		public string FirstName { get; set; }

		[JsonProperty("lastName")]
		public string LastName { get; set; }

		[JsonProperty("position")]
		[JsonConverter(typeof(StringEnumConverter))]
		public Position Position { get; set; }

		[JsonProperty("teamId")]
		public int? TeamId { get; set; } // not active if null

		[JsonProperty("number")]
		public int Number { get; set; }

		[JsonProperty("height")]
		public int Height { get; set; }

		[JsonProperty("weight")]
		public int Weight { get; set; }

		[JsonProperty("dateOfBirth")]
		public DateTime DateOfBirth { get; set; }

		[JsonProperty("college")]
		public string College { get; set; }

		public static Core.Models.PlayerData ToCoreEntity(PlayerDataJson json)
		{
			return new Core.Models.PlayerData
			{
				NflId = json.NflId,
				FirstName = json.FirstName,
				LastName = json.LastName,
				Position = json.Position,
				TeamId = json.TeamId,
				Number = json.Number,
				Height = json.Height,
				Weight = json.Weight,
				DateOfBirth = json.DateOfBirth,
				College = json.College
			};
		}
	}
}
