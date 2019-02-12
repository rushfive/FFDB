using R5.FFDB.Components.CoreData.Dynamic.Rosters;
using R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Update.Models;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Update.Mappers
{
	public interface IToCoreMapper : IAsyncMapper<PlayerUpdateVersioned, PlayerUpdate, string> { }

	public class ToCoreMapper : IToCoreMapper
	{
		private IRosterCache _rosterCache { get; }

		public ToCoreMapper(IRosterCache rosterCache)
		{
			_rosterCache = rosterCache;
		}

		public async Task<PlayerUpdate> MapAsync(PlayerUpdateVersioned versioned, string nflId)
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

			return new PlayerUpdate
			{
				FirstName = versioned.FirstName,
				LastName = versioned.LastName,
				Number = number,
				Position = position,
				Status = status
			};
		}
	}
}
