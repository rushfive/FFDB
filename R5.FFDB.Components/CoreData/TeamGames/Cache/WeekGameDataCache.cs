//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
//using R5.FFDB.Components.Configurations;
//using R5.FFDB.Components.CoreData.TeamGames.Models;
//using R5.FFDB.Components.Http;
//using R5.FFDB.Core;
//using R5.FFDB.Core.Entities;
//using R5.FFDB.Core.Models;
//using R5.Lib.Cache;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml.Linq;

//namespace R5.FFDB.Components.CoreData.TeamGames.Cache
//{
//	// todo move
//	public interface IResolvableAsyncCache<TKey, TValue>
//	{
//		Task<TValue> GetAsync(TKey key);
//	}

//	public interface IWeekGameDataCache : IResolvableAsyncCache<WeekInfo, WeekGameMatchups>
//	{
//		Task ResolveWeekAsync(WeekInfo week);
//		List<string> GetGameIds(WeekInfo week);
//		WeekInfo GetWeekForGame(string gameId);
//		Task<List<WeekGameMatchup>> GetMatchupsAsync(WeekInfo week);

//		// we would need a Week -> List<TeamWeekStats> if we want
//		// to consolidate this cache with TeamGameDataCache
//	}

//	public class WeekGameDataCache : ResolvableAsyncCache<WeekInfo, WeekGameMatchups>, IWeekGameDataCache
//	{
//		private Dictionary<WeekInfo, List<string>> _weekGamesMap { get; } = new Dictionary<WeekInfo, List<string>>();
//		private Dictionary<string, WeekInfo> _gameWeekMap { get; } = new Dictionary<string, WeekInfo>(StringComparer.OrdinalIgnoreCase);

//		private ILogger<TeamGameDataCache> _logger { get; }
//		private DataDirectoryPath _dataPath { get; }
//		private IWebRequestClient _webRequestClient { get; }
//		private ProgramOptions _programOptions { get; }

//		public WeekGameDataCache(
//			ILogger<TeamGameDataCache> logger,
//			DataDirectoryPath dataPath,
//			IWebRequestClient webRequestClient,
//			ProgramOptions programOptions)
//		{
//			_logger = logger;
//			_dataPath = dataPath;
//			_webRequestClient = webRequestClient;
//			_programOptions = programOptions;
//		}

//		public async Task ResolveWeekAsync(WeekInfo week)
//		{
//			await ResolveAsync(week);
//		}

//		public List<string> GetGameIds(WeekInfo week)
//		{
//			if (!_weekGamesMap.TryGetValue(week, out List<string> ids))
//			{
//				throw new InvalidOperationException($"Cache requires week '{week}' to be resolved first.");
//			}

//			return ids;
//		}

//		public WeekInfo GetWeekForGame(string gameId)
//		{
//			if (!_gameWeekMap.TryGetValue(gameId, out WeekInfo week))
//			{
//				throw new InvalidOperationException($"Cache hasn't resolved data for game '{gameId}'.");
//			}

//			return week;
//		}

//		public async Task<List<WeekGameMatchup>> GetMatchupsAsync(WeekInfo week)
//		{
//			WeekGameMatchups value = await GetAsync(week);

//			return value.Matchups
//				.Select(m => WeekGameMatchups.Matchup.ToCoreEntity(m, value.Week))
//				.ToList();
//		}

//		protected override async Task<WeekGameMatchups> ResolveAsync(WeekInfo week)
//		{
//			if (!TryGetFromDisk(week, out WeekGameMatchups matchups))
//			{
//				matchups = await FetchAsync(week);
//			}

//			_weekGamesMap[week] = matchups.Matchups.Select(m => m.NflGameId).ToList();

//			matchups.Matchups.ForEach(m => _gameWeekMap[m.NflGameId] = week);

//			return matchups;
//		}

//		private bool TryGetFromDisk(WeekInfo week, out WeekGameMatchups matchups)
//		{
//			matchups = null;

//			string filePath = null;// _dataPath.Static.WeekGameMatchups + $"{week.Season}-{week.Week}.json";

//			if (!File.Exists(filePath))
//			{
//				return false;
//			}

//			string serialized = File.ReadAllText(filePath);

//			matchups = JsonConvert.DeserializeObject<WeekGameMatchups>(serialized);

//			return true;
//		}

//		private async Task<WeekGameMatchups> FetchAsync(WeekInfo week)
//		{
//			string uri = Endpoints.Api.ScoreStripWeekGames(week);

//			string response = await _webRequestClient.GetStringAsync(uri, throttle: false);

//			XElement weekGameXml = XElement.Parse(response);

//			XElement gameNode = weekGameXml.Elements("gms").Single();

//			var result = new WeekGameMatchups
//			{
//				Week = week
//			};

//			foreach (XElement game in gameNode.Elements("g"))
//			{
//				int homeTeamId = TeamDataStore.GetIdFromAbbreviation(game.Attribute("h").Value, includePriorLookup: true);
//				int awayTeamId = TeamDataStore.GetIdFromAbbreviation(game.Attribute("v").Value, includePriorLookup: true);
//				string nflGameId = game.Attribute("eid").Value;
//				string gsisGameId = game.Attribute("gsis").Value;

//				var matchup = new WeekGameMatchups.Matchup
//				{
//					HomeTeamId = homeTeamId,
//					AwayTeamId = awayTeamId,
//					NflGameId = nflGameId,
//					GsisGameId = gsisGameId
//				};
				
//				result.Matchups.Add(matchup);
//			}

//			if (_programOptions.SaveToDisk)
//			{
//				string serialized = JsonConvert.SerializeObject(result);
//				File.WriteAllText(
//					//_dataPath.Static.WeekGameMatchups + $"{week.Season}-{week.Week}.json", 
//					null,
//					serialized);
//			}

//			return result;
//		}
//	}
//}
