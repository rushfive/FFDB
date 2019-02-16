using R5.FFDB.Components.CoreData.Dynamic.Rosters;
using R5.FFDB.Components.CoreData.Static.Players.Add.Sources.V1.Models;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.Players.Add.Sources.V1.Mappers
{
	public interface IToCoreMapper : IAsyncMapper<PlayerAddVersioned, PlayerAdd, string> { }

	public class ToCoreMapper : IToCoreMapper
	{
		private IRosterCache _rosterCache { get; }

		public ToCoreMapper(IRosterCache rosterCache)
		{
			_rosterCache = rosterCache;
		}

		public async Task<PlayerAdd> MapAsync(PlayerAddVersioned versioned, string nflId)
		{
			int? number = null;
			Position? position = null;
			RosterStatus? status = null;

			var playerData = await _rosterCache.GetPlayerDataAsync(nflId);
			if (playerData.HasValue)
			{
				number = playerData.Value.number;
				position = playerData.Value.position;
				status = playerData.Value.status;
			}

			return new PlayerAdd
			{
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
