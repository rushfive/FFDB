using R5.FFDB.Components.CoreData.Static.WeekMatchups.Sources.V1.Models;
using R5.FFDB.Core.Entities;
using System.Collections.Generic;

namespace R5.FFDB.Components.CoreData.Static.WeekMatchups.Sources.V1.Mappers
{
	public class ToCoreDataMapper : IMapper<WeekMatchupsVersionedModel, List<WeekGameMatchup>>
	{
		public List<WeekGameMatchup> Map(WeekMatchupsVersionedModel versionedModel)
		{
			var result = new List<WeekGameMatchup>();

			foreach (WeekMatchupsVersionedModel.Game game in versionedModel.Games)
			{
				result.Add(new WeekGameMatchup
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
