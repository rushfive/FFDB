using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1;
using R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1.Models;
using R5.FFDB.Components.Http;
using R5.FFDB.Core;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.Lib.Cache.AsyncLazyCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Dynamic.Rosters
{
	// Roster use cases:
	// 1. Updating players (dynamic stuff  like names, roster number position, etc)
	// 2. Updating Player-Team mappings (are they on a team?)
	//       - get nfl ids of everyone on a roster (used to resolve new players in UpdateRosterMappingsPipeline)
	//       -
	public interface IRosterCache
	{
		Task<List<Roster>> GetAsync();
		Task<List<string>> GetRosteredIdsAsync();
		Task<(int? number, Position? position, RosterStatus? status)?> GetPlayerDataAsync(string nflId);
	}

	public class RosterCache : IRosterCache
	{
		private const string _cacheKey = "rosters";

		private ILogger<RosterCache> _logger { get; }
		private IAsyncLazyCache _cache { get; }
		private IRosterSource _source { get; }
		private WebRequestThrottle _throttle { get; }

		public RosterCache(
			ILogger<RosterCache> logger,
			IAsyncLazyCache cache,
			IRosterSource source,
			WebRequestThrottle throttle)
		{
			_logger = logger;
			_cache = cache;
			_source = source;
			_throttle = throttle;
		}

		public async Task<List<Roster>> GetAsync()
		{
			RosterCacheData rosterData = await _cache.GetOrCreateAsync(_cacheKey, CreateCacheDataAsync);

			return rosterData.GetRosters();
		}

		public async Task<List<string>> GetRosteredIdsAsync()
		{
			RosterCacheData rosterData = await _cache.GetOrCreateAsync(_cacheKey, CreateCacheDataAsync);

			return rosterData.GetCurrentlyRosteredIds();
		}

		public async Task<(int? number, Position? position, RosterStatus? status)?> GetPlayerDataAsync(string nflId)
		{
			RosterCacheData rosterData = await _cache.GetOrCreateAsync(_cacheKey, CreateCacheDataAsync);

			return rosterData.GetPlayerData(nflId);
		}

		private async Task<RosterCacheData> CreateCacheDataAsync()
		{
			var data = new RosterCacheData();

			foreach (Team t in TeamDataStore.GetAll())
			{
				Roster roster = await _source.GetAsync(t);
				data.UpdateWith(roster);

				await _throttle.DelayAsync();
			}

			return data;
		}
	}
}
