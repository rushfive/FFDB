using Newtonsoft.Json;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1.Models
{
	public class RosterVersioned
	{
		[JsonProperty("teamId")]
		public int TeamId { get; set; }

		[JsonProperty("teamAbbreviation")]
		public string TeamAbbreviation { get; set; }

		[JsonProperty("players")]
		public List<Player> Players { get; set; }

		public static Roster ToCoreEntity(RosterVersioned model)
		{
			return new Roster
			{
				TeamId = model.TeamId,
				TeamAbbreviation = model.TeamAbbreviation,
				Players = model.Players.Select(Player.ToCoreEntity).ToList()
			};
		}

		public override string ToString()
		{
			return $"{TeamAbbreviation} Roster";
		}

		public class Player
		{
			[JsonProperty("nflId")]
			public string NflId { get; set; }

			[JsonProperty("number")]
			public int? Number { get; set; }

			[JsonProperty("position")]
			public Position Position { get; set; }

			[JsonProperty("status")]
			public RosterStatus Status { get; set; }

			public static RosterPlayer ToCoreEntity(Player model)
			{
				return new RosterPlayer
				{
					NflId = model.NflId,
					Number = model.Number,
					Position = model.Position,
					Status = model.Status
				};
			}
		}
	}
}
