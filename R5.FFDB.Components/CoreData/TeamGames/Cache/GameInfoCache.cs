using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.CoreData.TeamGames.NewTodoMove;
using R5.FFDB.Components.Http;
using R5.FFDB.Core.Models;
using R5.Lib.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace R5.FFDB.Components.CoreData.TeamGames.Cache
{
	// todo move
	public interface IResolvableAsyncCache<TKey, TValue>
	{
		Task<TValue> GetAsync(TKey key);
	}

	public interface IGameInfoCache : IResolvableAsyncCache<WeekInfo, List<string>>
	{
		WeekInfo GetWeekForGame(string gameId);
		Task<List<string>> GetGameIdsAsync(WeekInfo week);
	}

	public class GameInfoCache : ResolvableAsyncCache<WeekInfo, List<string>>, IGameInfoCache
	{
		private Dictionary<string, WeekInfo> _gameWeekMap { get; } = new Dictionary<string, WeekInfo>(StringComparer.OrdinalIgnoreCase);

		private ILogger<TeamGameDataCache> _logger { get; }
		private DataDirectoryPath _dataPath { get; }
		private IWebRequestClient _webRequestClient { get; }
		private ProgramOptions _programOptions { get; }

		public GameInfoCache(
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

		public Task<List<string>> GetGameIdsAsync(WeekInfo week)
		{
			return this.GetAsync(week);
		}

		public WeekInfo GetWeekForGame(string gameId)
		{
			if (!_gameWeekMap.TryGetValue(gameId, out WeekInfo week))
			{
				throw new InvalidOperationException($"Failed to find week for game '{gameId}'.");
			}

			return week;
		}

		protected override async Task<List<string>> ResolveAsync(WeekInfo week)
		{
			List<string> gameIds;
			if (TryGetFromDisk(week, out List<string> ids))
			{
				gameIds = ids;
			}
			else
			{
				gameIds = await FetchAsync(week);
			}

			gameIds.ForEach(id => _gameWeekMap[id] = week);

			return gameIds;
		}

		private bool TryGetFromDisk(WeekInfo week, out List<string> result)
		{
			result = null;

			string filePath = _dataPath.Static.TeamGameHistoryWeekGames + $"{week.Season}-{week.Week}.xml";

			if (!File.Exists(filePath))
			{
				return false;
			}

			XElement weekGameXml = XElement.Load(filePath);

			result = GetFromXmlElement(weekGameXml);
			
			return true;
		}

		private async Task<List<string>> FetchAsync(WeekInfo week)
		{
			string uri = Endpoints.Api.ScoreStripWeekGames(week.Season, week.Week);

			string response = await _webRequestClient.GetStringAsync(uri, throttle: false);

			XElement weekGameXml = XElement.Parse(response);

			return GetFromXmlElement(weekGameXml);
		}

		private List<string> GetFromXmlElement(XElement weekGameXml)
		{
			var result = new List<string>();

			XElement gameNode = weekGameXml.Elements("gms").Single();

			foreach (XElement game in gameNode.Elements("g"))
			{
				string gameId = game.Attribute("eid").Value;
				result.Add(gameId);
			}

			return result;
		}
	}
}
