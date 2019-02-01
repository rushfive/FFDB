﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.CoreData.TeamGames.Models;
using R5.FFDB.Components.Http;
using R5.FFDB.Components.SourceDataMappers.TeamGames;
using R5.FFDB.Core.Models;
using R5.Lib.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.TeamGames.NewTodoMove
{
	public interface ITeamGameDataCache
	{

	}

	// use cases:
	// - weekly team stats (eg pts scored per qtr, total yards, etc)
	// - resolve the teams players were on for given weeks

	// key = gameId
	public class TeamGameDataCache : ResolvableAsyncCache<string, TeamGameData>, ITeamGameDataCache
	{
		private ILogger<TeamGameDataCache> _logger { get; }
		private DataDirectoryPath _dataPath { get; }
		private IWebRequestClient _webRequestClient { get; }
		private ProgramOptions _programOptions { get; }

		public TeamGameDataCache(
			ILogger<TeamGameDataCache> logger,
			DataDirectoryPath dataPath,
			IWebRequestClient webRequestClient,
			ProgramOptions programOptions)
		{
			_logger = logger;
			_dataPath = dataPath;
			_webRequestClient = webRequestClient;
			_programOptions = programOptions;
		}

		public async Task<List<TeamGameData>> GetForWeekAsync(WeekInfo week)
		{
			throw new NotImplementedException();
		}

		// Returns a func that takes an nfl id, and returns the teams id
		public async Task<Func<string, int?>> GetPlayerTeamResolverAsync(WeekInfo week)
		{
			throw new NotImplementedException();
		}


		protected override async Task<TeamGameData> ResolveAsync(string gameId)
		{
			if (TryGetFromDisk(gameId, out TeamGameData value))
			{
				return value;
			}

			return await FetchAsync(gameId);
		}

		private bool TryGetFromDisk(string gameId, out TeamGameData value)
		{
			value = null;

			string filePath = _dataPath.Static.TeamGameHistoryGameStats + $"{gameId}.json";

			if (!File.Exists(filePath))
			{
				return false;
			}

			string serialized = File.ReadAllText(filePath);
			value = JsonConvert.DeserializeObject<TeamGameData>(serialized);
			return true;
		}


		// NEED:
		// - gsis to nfl id mapping
		// - week resolver (gameId -> week)
		private async Task<TeamGameData> FetchAsync(string gameId)
		{
			throw new NotImplementedException();
			//string uri = Endpoints.Api.GameCenterStats(gameId);
			
			//string response = await _webRequestClient.GetStringAsync(uri, throttle: false);

			//JObject json = JObject.Parse(response);
			//TeamGameData teamData = TeamGameData.FromGameStats(json, gameId, week, gsisNflIdMap);

			//if (_programOptions.SaveToDisk)
			//{
			//	string serialized = JsonConvert.SerializeObject(teamData);
			//	File.WriteAllText(_dataPath.Static.TeamGameHistoryGameStats + $"{gameId}.json", serialized);
			//}

			//return teamData;
		}
	}


}