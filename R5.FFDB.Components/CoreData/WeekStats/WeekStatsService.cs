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

namespace R5.FFDB.Components.CoreData.WeekStats
{
	public interface IWeekStatsService
	{
		Core.Models.WeekStats GetForWeek(WeekInfo week);
		List<string> GetNflIdsForWeek(WeekInfo week);
		//
		List<Core.Models.WeekStats> Get();

	}
	public class WeekStatsService : IWeekStatsService
	{
		private ILogger<WeekStatsService> _logger { get; }
		private DataDirectoryPath _dataPath { get; }
		private IPlayerWeekTeamMap _playerWeekTeamMap { get; }

		public WeekStatsService(
			ILogger<WeekStatsService> logger,
			DataDirectoryPath dataPath,
			IPlayerWeekTeamMap playerWeekTeamHistory)
		{
			_logger = logger;
			_dataPath = dataPath;
			_playerWeekTeamMap = playerWeekTeamHistory;
		}

		public List<string> GetNflIdsForWeek(WeekInfo week)
		{
			string path = _dataPath.Static.WeekStats + $"{week.Season}-{week.Week}.json";

			var json = JsonConvert.DeserializeObject<WeekStatsJson>(File.ReadAllText(path));

			WeekStatsGameJson games = json.Games.Single().Value;

			return games.Players.Keys.ToList();
		}

		public Core.Models.WeekStats GetForWeek(WeekInfo week)
		{
			return GetStats(week);
		}

		// pre per-week below

		public List<Core.Models.WeekStats> Get()
		{
			return DirectoryFilesResolver
				.GetWeeksFromJsonFiles(_dataPath.Static.WeekStats)
				.Select(w => GetStats(w))
				.ToList();
		}

		private Core.Models.WeekStats GetStats(WeekInfo week)
		{
			string path = _dataPath.Static.WeekStats + $"{week.Season}-{week.Week}.json";

			var json = JsonConvert.DeserializeObject<WeekStatsJson>(File.ReadAllText(path));

			return WeekStatsJson.ToCoreEntity(json, week, _playerWeekTeamMap.GetTeam);
		}
	}
}
