using DevTester.Testers;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using R5.FFDB.Components;
using R5.FFDB.Components.CoreData.PlayerProfile;
using R5.FFDB.Components.CoreData.PlayerProfile.Models;
using R5.FFDB.Components.CoreData.Roster;
using R5.FFDB.Components.CoreData.TeamData.Models;
using R5.FFDB.Components.CoreData.TeamGameHistory;
using R5.FFDB.Components.CoreData.TeamGameHistory.Models;
using R5.FFDB.Components.CoreData.WeekStats;
using R5.FFDB.Components.Mappers;
using R5.FFDB.Core.Models;
using R5.FFDB.Database;
using R5.FFDB.DbProviders.PostgreSql;
using R5.FFDB.DbProviders.PostgreSql.Models;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DevTester
{
	public class DevProgram
	{
		private static IServiceProvider _serviceProvider { get; set; }
		private static ILogger<DevProgram> _logger { get; set; }

		public static async Task Main(string[] args)
		{
			_serviceProvider = DevTestServiceProvider.Build();
			_logger = _serviceProvider.GetRequiredService<ILogger<DevProgram>>();
			var dataPath = _serviceProvider.GetRequiredService<DataDirectoryPath>();

			var weekStatsSource = _serviceProvider.GetRequiredService<IWeekStatsSource>();
			var stats = weekStatsSource.GetStats(new WeekInfo(2018, 14));

			var pm = stats.Players.Single(p => p.NflId == "2558125");

			WeekStatsSql sql = WeekStatsSql.FromCoreEntity(Guid.Empty,
				0, 2018, 14, pm.Stats);

			 var testttt = "test";














			//await PostgresTester.SetupTablesAsync(_serviceProvider);
			//await PostgresTester.InsertTeamsAsync(_serviceProvider);

			var teamSql = new TeamSql();
			var createTeam = teamSql.CreateTableCommand();
			var teamTblName = EntityInfoMap.TableName(typeof(TeamSql));

			var playerSql = new PlayerSql();
			var createPlayer = playerSql.CreateTableCommand();
			var playerTblName = EntityInfoMap.TableName(typeof(PlayerSql));

			var playerTeamMapSql = new PlayerTeamMapSql();
			var createPlayerTeamMap = playerTeamMapSql.CreateTableCommand();
			var playerTeamMapTblName = EntityInfoMap.TableName(typeof(PlayerTeamMapSql));

			var weekStatsSql = new WeekStatsSql();
			var createweekStats = weekStatsSql.CreateTableCommand();
			var weekStatsTblName = EntityInfoMap.TableName(typeof(WeekStatsSql));

			//var infos = teamSql.GetColumnInfos();
			//var create = SqlEntityCommandBuilder.CreateTable(typeof(TeamSql));
			//var create2 = SqlEntityCommandBuilder.CreateTable(typeof(TeamSql));

			//string tableName = teamSql.TableName();

			//string teamTableName = EntityInfoMap.TableName(typeof(TeamSql));

			var te = "t";



			// redownload all profiles
			//await UpdatePlayerProfileFilesAsync();

			//var weekTeamMap = _serviceProvider.GetRequiredService<PlayerWeekTeamMap>();

			//Dictionary<string, Dictionary<WeekInfo, int>> map
			//	= Timer.Time<Dictionary<string, Dictionary<WeekInfo, int>>>("build map",
			//	() => weekTeamMap.Get(), TimerUnit.Seconds);

			//var weekStatsSource = _serviceProvider.GetRequiredService<IWeekStatsSource>();
			//HashSet<string> playersFromWeekStats = weekStatsSource.GetAll()
			//	.SelectMany(s => s.Players)
			//	.Select(p => p.NflId)
			//	.ToHashSet();

			//Console.WriteLine($"{playersFromWeekStats.Count} total players from week stats.");

			//int notFound = 0;
			//foreach(string nflId in playersFromWeekStats)
			//{
			//	if (!map.ContainsKey(nflId))
			//	{
			//		Console.WriteLine($"Failed to find nfl id '{nflId}' in week team map.");
			//		notFound++;
			//	}
			//}

			//Console.WriteLine($"Failed to find a total of {notFound} players in map.");

			return;











			// get GSIS to NFL id mapping


			//var teamHistorySource = _serviceProvider.GetRequiredService<ITeamGameHistorySource>();
			//await teamHistorySource.FetchAndSaveAsync();



			//var postgresConfig = new PostgresConfig
			//{
			//	DatabaseName = "ffdb_test_1",
			//	Host = "localhost",
			//	Username = "ffdb",
			//	Password = "welc0me!"
			//};

			//var postgresProvider = new PostgresDbProvider(postgresConfig);
			//IDatabaseContext context = postgresProvider.GetContext();

			//await context.RunInitialSetupAsync();


			Console.ReadKey();
		}

		// REMOVE all existing files first to ensure updated copies
		private static async Task UpdatePlayerProfileFilesAsync()
		{
			var nflIdsToFetch = new HashSet<string>();

			var weekStatSource = _serviceProvider.GetRequiredService<IWeekStatsSource>();
			await weekStatSource.FetchAndSaveAsync();
			weekStatSource.GetAll()
				.SelectMany(s => s.Players)
				.ToList()
				.ForEach(p => nflIdsToFetch.Add(p.NflId));

			var rosterSource = _serviceProvider.GetRequiredService<IRosterSource>();
			List<Roster> rosters = await rosterSource.GetAsync();
			rosters
				.SelectMany(r => r.Players)
				.ToList()
				.ForEach(p => nflIdsToFetch.Add(p.NflId));

			var playerProfileSource = _serviceProvider.GetRequiredService<IPlayerProfileSource>();
			await playerProfileSource.FetchAndSaveAsync(nflIdsToFetch.ToList());
		}

		private static async Task UpdateAllSourceFilesAsync()
		{
			var weekStatsSource = _serviceProvider.GetRequiredService<IWeekStatsSource>();
			await weekStatsSource.FetchAndSaveAsync();
			var ids = weekStatsSource.GetAll()
				.SelectMany(s => s.Players)
				.Select(p => p.NflId)
				.ToList();


			var profileSource = _serviceProvider.GetRequiredService<IPlayerProfileSource>();
			await profileSource.FetchAndSaveAsync(ids);
		}












		private static Task FetchPlayerProfilesFromRostersAsync(bool downloadRosterPages)
		{
			try
			{
				IPlayerProfileTester tester = _serviceProvider.GetRequiredService<IPlayerProfileTester>();
				return tester.FetchSavePlayerProfilesAsync(downloadRosterPages);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "There was an error fetching player profile from roster.");
				throw;
			}
		}

		private static async Task<HtmlDocument> DownloadPageAsync(
			string pageUri, string filePath, bool skipFetch)
		{
			// Save 

			//string uri = "http://www.nfl.com/player/mikemitchell/238227/gamelogs?season=2018";
			//string filePath = @"D:\Repos\ffdb_data\temp\debug.html";

			if (!skipFetch)
			{
				var web = new HtmlWeb();
				HtmlDocument doc = await web.LoadFromWebAsync(pageUri);
				doc.Save(filePath);
			}


			// Read
			string html = File.ReadAllText(filePath);
			var page = new HtmlDocument();
			page.LoadHtml(html);

			return page;
		}
	}
}
