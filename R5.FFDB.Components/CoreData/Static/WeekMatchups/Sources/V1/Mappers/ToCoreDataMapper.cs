using R5.FFDB.Components.CoreData.Static.WeekMatchups.Sources.V1.Models;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.WeekMatchups.Sources.V1.Mappers
{
	public interface IToCoreDataMapper : IAsyncMapper<WeekMatchupsVersionedModel, List<WeekGameMatchup>, WeekInfo> { }

	public class ToCoreDataMapper : IToCoreDataMapper
	{
		public Task<List<WeekGameMatchup>> MapAsync(WeekMatchupsVersionedModel versionedModel, WeekInfo week)
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

			return Task.FromResult(result);
		}
	}
}
