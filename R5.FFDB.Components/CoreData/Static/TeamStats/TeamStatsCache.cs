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
	public interface ITeamStatsCache
	{
		Task<List<TeamWeekStats>> GetForWeekAsync(WeekInfo week);
		Task<Dictionary<string, int>> GetPlayerTeamMapAsync(WeekInfo week);
	}

	public class TeamStatsCache : ITeamStatsCache
	{
		private static string CacheKey(WeekInfo week) => $"team_stats_{week}";

		private ILogger<TeamStatsCache> _logger { get; }
		private IAsyncLazyCache _cache { get; }
		private ITeamStatsSource _source { get; }
		private IWeekMatchupsCache _weekMatchups { get; }

		public TeamStatsCache(
			ILogger<TeamStatsCache> logger,
			IAsyncLazyCache cache,
			ITeamStatsSource source,
			IWeekMatchupsCache weekMatchups)
		{
			_logger = logger;
			_cache = cache;
			_source = source;
			_weekMatchups = weekMatchups;
		}

		public async Task<List<TeamWeekStats>> GetForWeekAsync(WeekInfo week)
		{
			TeamStatsCacheData cacheData = await _cache.GetOrCreateAsync(CacheKey(week), () => CreateCacheDataAsync(week));

			return cacheData.GetStats();
		}

		public async Task<Dictionary<string, int>> GetPlayerTeamMapAsync(WeekInfo week)
		{
			TeamStatsCacheData cacheData = await _cache.GetOrCreateAsync(CacheKey(week), () => CreateCacheDataAsync(week));

			return cacheData.GetPlayerTeamMap();
		}

		private async Task<TeamStatsCacheData> CreateCacheDataAsync(WeekInfo week)
		{
			var data = new TeamStatsCacheData();

			List<string> gameIds = await _weekMatchups.GetGameIdsForWeekAsync(week);
			foreach(var id in gameIds)
			{
				TeamStatsSourceModel stats = await _source.GetAsync((id, week));
				data.UpdateWith(stats.HomeTeamStats);
				data.UpdateWith(stats.AwayTeamStats);
			}

			return data;
		}
	}
}
