using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using R5.FFDB.Components.CoreData.TeamGames;
using R5.FFDB.Components.CoreData.TeamGames.Models;
using R5.FFDB.Components.CoreData.WeekStats.Models;
using R5.FFDB.Components.Resolvers;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.SourceDataMappers.TeamGames
{
	public interface ITeamGamesDataMapper
	{
		Task Test();
		Task Test2();
		Task Test3();
		Task Test4();
	}
	public class TeamGamesDataMapper : ITeamGamesDataMapper
	{
		private DataDirectoryPath _dataPath { get; set; }
		private IDatabaseProvider _dbProvider { get; set; }
		private AvailableWeeksValue _availableWeeksValue { get; set; }
		private IPlayerWeekTeamResolverFactory _playerWeekTeamResolverFactory { get; }

		public TeamGamesDataMapper(
			DataDirectoryPath dataPath,
			IDatabaseProvider dbProvider,
			AvailableWeeksValue availableWeeksValue,
			IPlayerWeekTeamResolverFactory playerWeekTeamResolverFactory)
		{
			_dataPath = dataPath;
			_dbProvider = dbProvider;
			_availableWeeksValue = availableWeeksValue;
			_playerWeekTeamResolverFactory = playerWeekTeamResolverFactory;
		}

		public async Task Test4()
		{
			// test week stats
			var all = new List<Core.Entities.WeekStats>();
			var filePaths = DirectoryFilesResolver.GetFilePaths(@"D:\Repos\ffdb_data_2\week_stats\");

			List<Core.Models.WeekInfo> allWeeks = await _availableWeeksValue.GetAsync();
			foreach (var week in allWeeks)
			{
				Func<string, int?> weekTeamResolver = await _playerWeekTeamResolverFactory.GetForWeekAsync(week);

				string path = _dataPath.Static.WeekStats + $"{week.Season}-{week.Week}.json";

				var json = JsonConvert.DeserializeObject<WeekStatsJson>(File.ReadAllText(path));

				var item = WeekStatsJson.ToCoreEntity(json, week, weekTeamResolver);
				all.Add(item);
			}


			string t = "test";
		}

		public async Task Test3()
		{
			// load all to test mem usage

			var allData = new List<TeamGameMatchupStats>();

			var filePaths = DirectoryFilesResolver.GetFilePaths(@"D:\Repos\ffdb_data_2\team_game_history\game_stats2\");

			foreach(var file in filePaths)
			{
				TeamGameMatchupStats json = JsonConvert.DeserializeObject<TeamGameMatchupStats>(
					File.ReadAllText(file));
				allData.Add(json);
			}

			string t = "test";
		}

		// get and save all as new model
		public async Task Test2()
		{
			Dictionary<string, string> gsisNflIdMap = await GetGsisToNflIdMapAsync();

			List<Core.Models.WeekInfo> allWeeks = await _availableWeeksValue.GetAsync();
			foreach(var week in allWeeks)
			{
				List<string> gameIds = TeamGamesUtil.GetGameIdsForWeek(week, _dataPath);
				foreach(var gameId in gameIds)
				{
					// try converting to new model FROM disk (this could be directly from a web response as well)
					JObject json = JObject.Parse(File.ReadAllText(_dataPath.Static.TeamGameStats + $"{gameId}.json"));

					var teamData = TeamGameMatchupStats.FromGameStats(json, gameId, week, gsisNflIdMap);
					string serializedJson = JsonConvert.SerializeObject(teamData);

					File.WriteAllText(@"D:\Repos\ffdb_data_2\team_game_history\game_stats2\" + $"{gameId}.json", serializedJson);
				}
			}
		}

		public async Task Test()
		{
			
			// 2018123015.json
			JObject json = JObject.Parse(File.ReadAllText(_dataPath.Static.TeamGameStats + "2018123015.json"));

			// dependencies
			Dictionary<string, string> gsisNflIdMap = await GetGsisToNflIdMapAsync(); // player week team
																					  // gameId and teamType (home or away) required to parse an inner jobject
			string gameId = "2018123015";

			var teamData = TeamGameMatchupStats.FromGameStats(json, gameId, new WeekInfo(2018,5), gsisNflIdMap);

			string serializedJson = JsonConvert.SerializeObject(teamData);

			File.WriteAllText(@"D:\Repos\ffdb_data_2\team_game_history\game_stats2\2018123015.json", serializedJson);
		}

		private async Task<Dictionary<string, string>> GetGsisToNflIdMapAsync()
		{
			IDatabaseContext dbContext = _dbProvider.GetContext();
			List<Player> players = await dbContext.Player.GetAllAsync();
			return players.ToDictionary(p => p.GsisId, p => p.NflId);
		}

		private void AddForTeam()
		{

		}
	}

	// todo move - filename should be nfl game id
	
}
