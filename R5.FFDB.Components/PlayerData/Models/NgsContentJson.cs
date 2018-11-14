using Newtonsoft.Json;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.FFDB.Components.PlayerData.Models
{
	public class NgsContentJson
	{
		[JsonProperty("games")]
		public Dictionary<string, NgsContentGameJson> Games { get; set; }

		public static NgsContentPlayer ToEntity(NgsContentJson model)
		{
			NgsContentGameJson game = model.Games.Single().Value;
			NgsContentPlayerJson player = game.Players.Single().Value;

			int? teamId = string.IsNullOrWhiteSpace(player.NflTeamId)
				? null
				: (int?)int.Parse(player.NflTeamId);

			return new NgsContentPlayer
			{
				NflId = player.NflId,
				FirstName = player.FirstName,
				LastName = player.LastName,
				Position = Enum.Parse<Position>(player.Position),
				TeamId = teamId
			};
		}
	}

	public class NgsContentGameJson
	{
		[JsonProperty("players")]
		public Dictionary<string, NgsContentPlayerJson> Players { get; set; }
	}

	public class NgsContentPlayerJson
	{
		[JsonProperty("playerId")]
		public string NflId { get; set; }

		[JsonProperty("firstName")]
		public string FirstName { get; set; }

		[JsonProperty("lastName")]
		public string LastName { get; set; }

		[JsonProperty("position")]
		public string Position { get; set; }

		[JsonProperty("nflTeamId")]
		public string NflTeamId { get; set; }
	}
}
