using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.Static.TeamStats.Models;
using R5.FFDB.Components.CoreData.Static.TeamStats.Sources.V1;
using R5.FFDB.Components.CoreData.Static.WeekMatchups;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.Lib.Cache.AsyncLazyCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.TeamStats
{
	public interface ITeamWeekStatsCache
	{
		Task<List<TeamWeekStats>> GetForWeekAsync(WeekInfo week);
		Task<Dictionary<string, int>> GetPlayerTeamMapAsync(WeekInfo week);
	}

	public class TeamWeekStatsCache : ITeamWeekStatsCache
	{
		private static string CacheKey(WeekInfo week) => $"team_week_stats_{week}";

		private ILogger<TeamWeekStatsCache> _logger { get; }
		private IAsyncLazyCache _cache { get; }
		private ITeamWeekStatsSource _source { get; }
		private IWeekMatchupsCache _weekMatchups { get; }

		public TeamWeekStatsCache(
			ILogger<TeamWeekStatsCache> logger,
			IAsyncLazyCache cache,
			ITeamWeekStatsSource source,
			IWeekMatchupsCache weekMatchups)
		{
			_logger = logger;
			_cache = cache;
			_source = source;
			_weekMatchups = weekMatchups;
		}

		public async Task<List<TeamWeekStats>> GetForWeekAsync(WeekInfo week)
		{
			TeamWeekStatsCacheData cacheData = await _cache.GetOrCreateAsync(CacheKey(week), () => CreateCacheDataAsync(week));

			return cacheData.GetStats();
		}

		public async Task<Dictionary<string, int>> GetPlayerTeamMapAsync(WeekInfo week)
		{
			TeamWeekStatsCacheData cacheData = await _cache.GetOrCreateAsync(CacheKey(week), () => CreateCacheDataAsync(week));

			return cacheData.GetPlayerTeamMap();
		}

		private async Task<TeamWeekStatsCacheData> CreateCacheDataAsync(WeekInfo week)
		{
			var data = new TeamWeekStatsCacheData();

			List<string> gameIds = await _weekMatchups.GetGameIdsForWeekAsync(week);
			foreach(var id in gameIds)
			{
				SourceResult<TeamWeekStatsSourceModel> result = await _source.GetAsync((id, week));
				data.UpdateWith(result.Value.HomeTeamStats);
				data.UpdateWith(result.Value.AwayTeamStats);
			}

			return data;
		}
	}
}
