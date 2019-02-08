using R5.FFDB.Components.CoreData.Static.WeekGameMap.Sources.V1.Models;
using R5.FFDB.Core.Entities;
using System.Collections.Generic;

namespace R5.FFDB.Components.CoreData.Static.WeekGameMap.Sources.V1.Mappers
{
	public class ToCoreDataMapper : IMapper<WeekGamesVersionedModel, List<WeekGameMapping>>
	{
		public List<WeekGameMapping> Map(WeekGamesVersionedModel versionedModel)
		{
			var result = new List<WeekGameMapping>();

			foreach (WeekGamesVersionedModel.Game game in versionedModel.Games)
			{
				result.Add(new WeekGameMapping
				{
					Week = versionedModel.Week,
					HomeTeamId = game.HomeTeamId,
					AwayTeamId = game.AwayTeamId,
					NflGameId = game.NflGameId,
					GsisGameId = game.GsisGameId
				});
			}

			return result;
		}
	}
}
