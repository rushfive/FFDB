using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.Static.WeekMatchups.Sources.V1;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.Lib.Cache.AsyncLazyCache;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.WeekMatchups
{
	public interface IWeekMatchupsCache
	{
		Task<List<WeekMatchup>> GetMatchupsForWeekAsync(WeekInfo week);
		Task<List<string>> GetGameIdsForWeekAsync(WeekInfo week);
	}

	public class WeekMatchupsCache : IWeekMatchupsCache
	{
		public static string CacheKey(WeekInfo week) => $"week_matchups_{week}";

		private ILogger<WeekMatchupsCache> _logger { get; }
		private IAsyncLazyCache _cache { get; }
		private IWeekMatchupSource _source { get; }

		public WeekMatchupsCache(
			ILogger<WeekMatchupsCache> logger,
			IAsyncLazyCache cache,
			IWeekMatchupSource source)
		{
			_logger = logger;
			_cache = cache;
			_source = source;
		}

		public async Task<List<WeekMatchup>> GetMatchupsForWeekAsync(WeekInfo week)
		{
			SourceResult<List<WeekMatchup>> result = await _cache.GetOrCreateAsync(CacheKey(week), () => _source.GetAsync(week));

			return result.Value;
		}

		public async Task<List<string>> GetGameIdsForWeekAsync(WeekInfo week)
		{
			SourceResult<List<WeekMatchup>> result = await _cache.GetOrCreateAsync(CacheKey(week), () => _source.GetAsync(week));

			return result.Value.Select(m => m.NflGameId).ToList();
		}
	}
}
