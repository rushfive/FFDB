using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.Static.WeekGameMap.Sources.V1;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.Lib.Cache.AsyncLazyCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.WeekGameMap
{
	public interface IWeekGameMapCache
	{
		Task<List<string>> GetGameIdsForWeekAsync(WeekInfo week);
		Task<List<WeekGameMatchup>> GetMatchupsForWeekAsync(WeekInfo week);
	}

	public class WeekGameMapCache : IWeekGameMapCache
	{
		private static string CacheKey(WeekInfo week) => $"weekGameMap_{week}";
		private readonly SemaphoreSlim _exclusiveLock = new SemaphoreSlim(1, 1);

		private ILogger<WeekGameMapCache> _logger { get; }
		private IAsyncLazyCache _cache { get; }
		private IWeekGameMapSource _source { get; }

		private Dictionary<WeekInfo, List<WeekGameMapping>> _weekGamesMap { get; }
			= new Dictionary<WeekInfo, List<WeekGameMapping>>();

		public WeekGameMapCache(
			ILogger<WeekGameMapCache> logger,
			IAsyncLazyCache cache,
			IWeekGameMapSource source)
		{
			_logger = logger;
			_cache = cache;
			_source = source;
		}

		public async Task<List<string>> GetGameIdsForWeekAsync(WeekInfo week)
		{
			List<WeekGameMapping> mappings = await _cache.GetOrCreateAsync(CacheKey(week), () => _source.GetAsync(week));

			return mappings.Select(m => m.NflGameId).ToList();
		}

		public async Task<List<WeekGameMatchup>> GetMatchupsForWeekAsync(WeekInfo week)
		{
			List<WeekGameMapping> mappings = await _cache.GetOrCreateAsync(CacheKey(week), () => _source.GetAsync(week));



			throw new NotImplementedException();
		}
	}
}
