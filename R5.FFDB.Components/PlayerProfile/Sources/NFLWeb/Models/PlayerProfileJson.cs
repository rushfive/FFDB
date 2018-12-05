using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.PlayerProfile.Sources.NFLWeb.Models
{
	// static data retrieved from api and scraping
	public class PlayerProfileJson
	{
		[JsonProperty("nflId")]
		public string NflId { get; set; }

		[JsonProperty("esbId")]
		public string EsbId { get; set; }

		[JsonProperty("gsisId")]
		public string GsisId { get; set; }

		[JsonProperty("pictureUri")]
		public string PictureUri { get; set; }

		[JsonProperty("firstName")]
		public string FirstName { get; set; }

		[JsonProperty("lastName")]
		public string LastName { get; set; }

		//[JsonProperty("position")]
		//[JsonConverter(typeof(StringEnumConverter))]
		//public Position Position { get; set; }

		//[JsonProperty("teamId")]
		//public int? TeamId { get; set; } // not active if null

		//[JsonProperty("number")]
		//public int Number { get; set; }

		[JsonProperty("height")]
		public int Height { get; set; }

		[JsonProperty("weight")]
		public int Weight { get; set; }

		[JsonProperty("dateOfBirth")]
		public DateTime DateOfBirth { get; set; }

		[JsonProperty("college")]
		public string College { get; set; }

		public static Core.Models.PlayerProfile ToCoreEntity(PlayerProfileJson json)
		{
			return new Core.Models.PlayerProfile
			{
				NflId = json.NflId,
				EsbId = json.EsbId,
				GsisId = json.GsisId,
				PictureUri = json.PictureUri,
				FirstName = json.FirstName,
				LastName = json.LastName,
				Height = json.Height,
				Weight = json.Weight,
				DateOfBirth = json.DateOfBirth,
				College = json.College
			};
		}
	}
}
