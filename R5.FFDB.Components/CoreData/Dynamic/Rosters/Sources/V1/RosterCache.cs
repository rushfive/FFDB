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

namespace R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1
{
	public interface IRosterCache
	{
		Task<(int? number, Position? position, RosterStatus? status)?> GetPlayerDataAsync(string nflId);
	}
	//singleton!
	public class RosterCache : IRosterCache
	{
		private const string _cacheKey = "rosters";
		private readonly SemaphoreSlim _exclusiveLock = new SemaphoreSlim(1, 1);

		private IAsyncLazyCache _cache { get; }
		private IRosterSource _source { get; }

		private List<Roster> _rosters { get; set; }
		private Dictionary<string, (int?, Position?, RosterStatus?)> _playerDataMap { get; set; }

		public RosterCache(
			IAsyncLazyCache cache,
			IRosterSource source)
		{
			_cache = cache;
			_source = source;
		}


		// does this need to allow returning null if doesnt exist??
		public async Task<(int? number, Position? position, RosterStatus? status)?> GetPlayerDataAsync(string nflId)
		{
			await _exclusiveLock.WaitAsync();
			try
			{
				if (_rosters == null)
				{
					await ResolveCacheDataAsync();
				}
			}
			finally
			{
				_exclusiveLock.Release();
			}
			
			if (_playerDataMap.TryGetValue(nflId, out (int?, Position?, RosterStatus?) data))
			{
				return data;
			}

			return null;
		}

		private async Task ResolveCacheDataAsync()
		{
			List<Roster> rosters = await _cache.GetOrCreateAsync(_cacheKey, GetRostersAsync);

			var playerDataMap = new Dictionary<string, (int?, Position?, RosterStatus?)>(StringComparer.OrdinalIgnoreCase);
			foreach (var p in rosters.SelectMany(r => r.Players))
			{
				playerDataMap[p.NflId] = (p.Number, p.Position, p.Status);
			}

			_rosters = rosters;
			_playerDataMap = playerDataMap;
		}

		private async Task<List<Roster>> GetRostersAsync()
		{
			List<Team> teams = TeamDataStore.GetAll();

			// temp: take
			var fetchTasks = teams.Take(2).Select(t => Task.Run(() =>
			{
				Console.WriteLine($"running TEAM/ROSTER fetch task for '{t}'.");

				return _source.GetAsync(t);
			}));

			Roster[] rosters = await Task.WhenAll(fetchTasks);
			return rosters.ToList();
		}
	}
}
