﻿using R5.FFDB.Components.CoreData.Static.WeekMatchups.Sources.V1.Models;
using R5.FFDB.Core;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace R5.FFDB.Components.CoreData.Static.WeekMatchups.Sources.V1.Mappers
{
	// parses XML response from NFLs score strip endpoint:
	// http://www.nfl.com/ajax/scorestrip?season={season}&seasonType=REG&week={week}

	public interface IToVersionedMapper : IAsyncMapper<string, WeekMatchupsVersioned, WeekInfo> { }
	
	public class ToVersionedMapper : IToVersionedMapper
	{
		public Task<WeekMatchupsVersioned> MapAsync(string httpResponse, WeekInfo week)
		{
			XElement weekGameXml = XElement.Parse(httpResponse);

			XElement gamesNode = weekGameXml.Elements("gms").Single();

			var model = new WeekMatchupsVersioned
			{
				Week = week,
				Games = new List<WeekMatchupsVersioned.Game>()
			};

			foreach (XElement game in gamesNode.Elements("g"))
			{
				int homeTeamId = Teams.GetIdFromAbbreviation(game.Attribute("h").Value, includePriorLookup: true);
				int awayTeamId = Teams.GetIdFromAbbreviation(game.Attribute("v").Value, includePriorLookup: true);
				string nflGameId = game.Attribute("eid").Value;
				string gsisGameId = game.Attribute("gsis").Value;

				var matchup = new WeekMatchupsVersioned.Game
				{
					HomeTeamId = homeTeamId,
					AwayTeamId = awayTeamId,
					NflGameId = nflGameId,
					GsisGameId = gsisGameId
				};

				model.Games.Add(matchup);
			}

			return Task.FromResult(model);
		}
	}
}
