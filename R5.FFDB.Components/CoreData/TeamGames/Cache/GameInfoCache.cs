using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.CoreData.TeamGames.NewTodoMove;
using R5.FFDB.Components.Http;
using R5.FFDB.Core.Models;
using R5.Lib.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace R5.FFDB.Components.CoreData.TeamGames.Cache
{
	// gameId -> week
	// get game ids for week (currently in TeamGamesUtil)
	public interface IGameInfoCache
	{
		Task<WeekInfo> GetWeekAsync(string gameId);
		Task<List<string>> GetGameIdsAsync(WeekInfo week);
	}

	public class GameInfoCache : ResolvableAsyncCache<WeekInfo, List<string>>, IGameInfoCache
	{
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

		public Task<WeekInfo> GetWeekAsync(string gameId)
		{
			throw new NotImplementedException();
		}

		protected override Task<List<string>> ResolveAsync(WeekInfo week)
		{
			throw new NotImplementedException();

			string filePath = _dataPath.Static.TeamGameHistoryWeekGames + $"{week.Season}-{week.Week}.xml";

			if (File.Exists(filePath))
			{
				_logger.LogDebug($"Week games file already exists for {week}. Will not fetch.");
				return;
			}
		}

		private bool TryGetFromDisk(WeekInfo week, out List<string> result)
		{
			result = null;

			string filePath = _dataPath.Static.TeamGameHistoryWeekGames + $"{week.Season}-{week.Week}.xml";

			if (File.Exists(filePath))
			{
				result = new List<string>();

				XElement weekGameXml = XElement.Load(filePath);

				XElement gameNode = weekGameXml.Elements("gms").Single();

				foreach (XElement game in gameNode.Elements("g"))
				{
					string gameId = game.Attribute("eid").Value;
					result.Add(gameId);
				}

				return result;

				return true;
			}

			return false;
		}
	}
}
