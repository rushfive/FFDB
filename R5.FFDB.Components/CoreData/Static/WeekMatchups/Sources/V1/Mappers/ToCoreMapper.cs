using R5.FFDB.Components.CoreData.Static.WeekMatchups.Sources.V1.Models;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.WeekMatchups.Sources.V1.Mappers
{
	public interface IToCoreMapper : IAsyncMapper<WeekMatchupsVersioned, List<WeekMatchup>, WeekInfo> { }

	public class ToCoreMapper : IToCoreMapper
	{
		public Task<List<WeekMatchup>> MapAsync(WeekMatchupsVersioned versionedModel, WeekInfo week)
		{
			var result = new List<WeekMatchup>();

			foreach (WeekMatchupsVersioned.Game game in versionedModel.Games)
			{
				result.Add(new WeekMatchup
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
