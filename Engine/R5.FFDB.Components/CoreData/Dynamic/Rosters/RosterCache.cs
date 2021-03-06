﻿using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1;
using R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1.Models;
using R5.FFDB.Components.Http;
using R5.FFDB.Core;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using R5.Internals.Caching.Caches;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Dynamic.Rosters
{
	public interface IRosterCache
	{
		Task<List<Roster>> GetAsync();
		Task<List<string>> GetRosteredIdsAsync();
		Task<(int? number, Position? position, RosterStatus? status)?> GetPlayerDataAsync(string nflId);
	}

	public class RosterCache : IRosterCache
	{
		private const string _cacheKey = "rosters";

		private IAppLogger _logger { get; }
		private IAsyncLazyCache _cache { get; }
		private IRosterSource _source { get; }
		private WebRequestThrottle _throttle { get; }
		private ProgramOptions _programOptions { get; }
		private DataDirectoryPath _dataPath { get; }

		public RosterCache(
			IAppLogger logger,
			IAsyncLazyCache cache,
			IRosterSource source,
			WebRequestThrottle throttle,
			ProgramOptions programOptions,
			DataDirectoryPath dataPath)
		{
			_logger = logger;
			_cache = cache;
			_source = source;
			_throttle = throttle;
			_programOptions = programOptions;
			_dataPath = dataPath;
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

			_logger.LogInformation("Resolving rosters for all teams.");

			foreach (Team t in Teams.GetAll())
			{
				bool shouldThrottle = false;
				var versionedFilePath = _source.GetVersionedFilePath(t);

				if (!_programOptions.SkipRosterFetch)
				{
					File.Delete(versionedFilePath);
					shouldThrottle = true;
				}
				else if (!File.Exists(versionedFilePath))
				{
					shouldThrottle = true;
				}

				SourceResult<Roster> roster = await _source.GetAsync(t);

				if (roster.FetchedFromWeb)
				{
					_logger.LogInformation($"Fetched roster for '{t}' from web.");
				}
				else
				{
					_logger.LogInformation($"Retrieved saved roster for '{t}' from disk.");
				}

				data.UpdateWith(roster.Value);
				
				if (shouldThrottle)
				{
					await _throttle.DelayAsync();
				}
			}

			return data;
		}
	}
}
