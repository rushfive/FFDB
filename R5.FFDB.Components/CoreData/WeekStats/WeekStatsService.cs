using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R5.FFDB.Components.CoreData.TeamGameHistory;
using R5.FFDB.Components.CoreData.WeekStats.Models;
using R5.FFDB.Components.Resolvers;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.WeekStats
{
	public interface IWeekStatsService
	{
		List<string> GetNflIdsForWeek(WeekInfo week);
		Task<Core.Models.WeekStats> GetForWeekAsync(WeekInfo week);
		
		//
		Task<List<Core.Models.WeekStats>> GetAsync();

	}
	public class WeekStatsService : IWeekStatsService
	{
		private ILogger<WeekStatsService> _logger { get; }
		private DataDirectoryPath _dataPath { get; }
		private IPlayerWeekTeamResolverFactory _playerWeekTeamResolverFactory { get; }

		public WeekStatsService(
			ILogger<WeekStatsService> logger,
			DataDirectoryPath dataPath,
			IPlayerWeekTeamResolverFactory playerWeekTeamResolverFactory)
		{
			_logger = logger;
			_dataPath = dataPath;
			_playerWeekTeamResolverFactory = playerWeekTeamResolverFactory;
		}

		public List<string> GetNflIdsForWeek(WeekInfo week)
		{
			string path = _dataPath.Static.WeekStats + $"{week.Season}-{week.Week}.json";

			var json = JsonConvert.DeserializeObject<WeekStatsJson>(File.ReadAllText(path));

			WeekStatsGameJson games = json.Games.Single().Value;

			return games.Players.Keys.ToList();
		}

		public async Task<Core.Models.WeekStats> GetForWeekAsync(WeekInfo week)
		{
			Func<string, int?> weekTeamResolver = await _playerWeekTeamResolverFactory.GetForWeekAsync(week);

			return GetStats(week, weekTeamResolver);
		}

		// pre per-week below

		public async Task<List<Core.Models.WeekStats>> GetAsync()
		{
			var result = new List<Core.Models.WeekStats>();
			
			foreach(WeekInfo week in DirectoryFilesResolver.GetWeeksFromJsonFiles(_dataPath.Static.WeekStats))
			{
				Func<string, int?> weekTeamResolver = await _playerWeekTeamResolverFactory.GetForWeekAsync(week);
				result.Add(GetStats(week, weekTeamResolver));
			}

			return result;
		}

		private Core.Models.WeekStats GetStats(WeekInfo week, Func<string, int?> weekTeamResolver)
		{
			string path = _dataPath.Static.WeekStats + $"{week.Season}-{week.Week}.json";

			var json = JsonConvert.DeserializeObject<WeekStatsJson>(File.ReadAllText(path));

			return WeekStatsJson.ToCoreEntity(json, week, weekTeamResolver);
		}
	}
}
