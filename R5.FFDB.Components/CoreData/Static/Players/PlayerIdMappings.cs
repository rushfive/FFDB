using R5.FFDB.Core.Database;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.Players
{
	public interface IPlayerIdMappings
	{
		Task<Dictionary<string, string>> GetGsisToNflMapAsync();
		Task<Dictionary<string, Guid>> GetNflToIdMapAsync();
	}

	public class PlayerIdMappings : IPlayerIdMappings
	{
		private IDatabaseProvider _dbProvider { get; }

		public PlayerIdMappings(IDatabaseProvider dbProvider)
		{
			_dbProvider = dbProvider;
		}

		public async Task<Dictionary<string, string>> GetGsisToNflMapAsync()
		{
			IDatabaseContext dbContext = _dbProvider.GetContext();

			List<Player> players = await dbContext.Player.GetAllAsync();

			return players.ToDictionary(p => p.GsisId, p => p.NflId);
		}

		public async Task<Dictionary<string, Guid>> GetNflToIdMapAsync()
		{
			IDatabaseContext dbContext = _dbProvider.GetContext();

			List<Player> players = await dbContext.Player.GetAllAsync();

			return players.ToDictionary(p => p.NflId, p => p.Id);
		}
	}
}
