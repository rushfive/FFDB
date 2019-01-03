﻿using DevTester.Testers;
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
using R5.FFDB.Components.CoreData.TeamGameHistory.Values;
using R5.FFDB.Components.CoreData.WeekStats;
using R5.FFDB.Components.Resolvers;
using R5.FFDB.Core.Models;
using R5.FFDB.Database;
using R5.FFDB.DbProviders.PostgreSql;
using R5.FFDB.DbProviders.PostgreSql.DatabaseProvider;
using R5.FFDB.DbProviders.PostgreSql.Models;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities.WeekStats;
using R5.FFDB.Engine;
using R5.FFDB.Engine.Source;
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
			_logger = _serviceProvider.GetService<ILogger<DevProgram>>();
			//var dataPath = _serviceProvider.GetRequiredService<DataDirectoryPath>();

			var rSource = _serviceProvider.GetRequiredService<IRosterSource>();
			await rSource.FetchAsync();


			//await InitDbAsync();
			await TestUpdateAsync(new WeekInfo(2010, 1));

			return;
			

			var dbProvider = _serviceProvider.GetRequiredService<IDatabaseProvider>();
			IDatabaseContext dbContext = dbProvider.GetContext();

			//await InitialSetupTestAsync();

			return;

			
			Console.ReadKey();
		}

		private static async Task InitDbAsync()
		{
			var dbProvider = _serviceProvider.GetRequiredService<IDatabaseProvider>();
			IDatabaseContext dbContext = dbProvider.GetContext();
			await dbContext.InitializeAsync();
			await dbContext.Team.AddTeamsAsync();
		}

		// things that need to happen before this:
		// assumes db initializes with tbls and tms
		// get latest rosters
		private static async Task TestUpdateAsync(WeekInfo week)
		{
			var wks = new List<WeekInfo> { week };

			var dbProvider = _serviceProvider.GetRequiredService<IDatabaseProvider>();
			IDatabaseContext dbContext = dbProvider.GetContext();
			var tgHistorySource = _serviceProvider.GetRequiredService<ITeamGameHistorySource>();
			var wsSource = _serviceProvider.GetRequiredService<IWeekStatsSource>();
			var wsSvc = _serviceProvider.GetRequiredService<IWeekStatsService>();
			var pSource = _serviceProvider.GetRequiredService<IPlayerProfileSource>();
			var pSvc = _serviceProvider.GetRequiredService<IPlayerProfileService>();
			//var rSource = _serviceProvider.GetRequiredService<IRosterSource>();
			var rSvc = _serviceProvider.GetRequiredService<IRosterService>();
			
			await tgHistorySource.FetchForWeeksAsync(wks);
			await wsSource.FetchForWeeksAsync(wks);
			
			HashSet<string> existingNflIds = (await dbContext.Player.GetExistingNflIdsAsync()).ToHashSet();
			List<string> idsFromWeek = wsSvc.GetNflIdsForWeek(week);
			var idsToFetch = idsFromWeek.Where(id => !existingNflIds.Contains(id)).ToList();
			await pSource.FetchAsync(idsToFetch);


			var t = "test";
		}





		private static async Task TestEngineInitialSetupAsync()
		{
			var setup = new EngineSetup();

			setup
				.SetRootDataDirectoryPath(@"D:\Repos\ffdb_data\")
				.UsePostgreSql(new PostgresConfig
				{
					DatabaseName = "ffdb_test_1",
					Host = "localhost",
					Username = "ffdb",
					Password = "welc0me!"
				});

			setup.WebRequest
				.SetThrottle(1000)
				.AddDefaultBrowserHeaders();

			setup.Logging
				.SetLogDirectory(@"D:\Repos\ffdb_data\dev_test_logs\")
				.SetRollingInterval(RollingInterval.Day)
				.SetLogLevel(LogEventLevel.Debug);

			FfdbEngine engine = setup.Create();
			await engine.RunInitialSetupAsync();
		}











		// TODO: this will all be copied to the ENGINEs initial setup method
		//       after done and tested
		//private static async Task InitialSetupTestAsync()
		//{
		//	_logger.LogInformation("Running initial setup..");

		//	var dbProvider = _serviceProvider.GetRequiredService<IDatabaseProvider>();
		//	IDatabaseContext dbContext = dbProvider.GetContext();
		//	_logger.LogInformation($"Will run using database provider '{dbProvider.GetType().Name}'.");

		//	//await dbContext.InitializeAsync();
		//	//await dbContext.Team.AddTeamsAsync();

		//	var sourcesResolver = _serviceProvider.GetRequiredService<CoreDataSourcesResolver>();
		//	CoreDataSources sources = await sourcesResolver.GetAsync();

		//	// ALSO: need to fetch latest team game history
		//	//await sources.TeamGameHistory.FetchAndSaveAsync();

		//	var gameStatsParser = _serviceProvider.GetRequiredService<IGameStatsParser>();
		//	gameStatsParser.ParseFilesToMapValues();

		//	await sources.Roster.FetchAndSaveAsync();
		//	var rosterService = _serviceProvider.GetRequiredService<IRosterService>();
		//	List<Roster> rosters = rosterService.Get();

		//	await sources.WeekStats.FetchAndSaveAsync();
		//	var weekStatsService = _serviceProvider.GetRequiredService<IWeekStatsService>();
		//	List<WeekStats> weekStats = weekStatsService.Get()
		//		.OrderBy(ws => ws.Week)
		//		.ToList();

		//	_logger.LogInformation("Fetching player profiles for players resolved from roster and week stats.");

		//	await sources.PlayerProfile.FetchAndSaveAsync();

		//	_logger.LogInformation("Beginning persisting of player profiles to database..");

		//	var playerProfileService = _serviceProvider.GetRequiredService<IPlayerProfileService>();
		//	List<PlayerProfile> players = playerProfileService.Get();
		//	await dbContext.Player.AddAsync(players, rosters);

		//	_logger.LogInformation("Beginning persisting of player-team mappings to database..");

		//	await dbContext.Team.UpdateRostersAsync(rosters);

		//	await dbContext.Stats.UpdateWeeksAsync(weekStats);

		//	var gameStatsService = _serviceProvider.GetRequiredService<ITeamGameStatsService>();
		//	List<TeamWeekStats> teamGameStats = gameStatsService.Get();
		//	await dbContext.Team.UpdateGameStatsAsync(teamGameStats);
		//}


















		// REMOVE all existing files first to ensure updated copies
		//private static async Task UpdatePlayerProfileFilesAsync()
		//{
		//	var nflIdsToFetch = new HashSet<string>();

		//	var weekStatSource = _serviceProvider.GetRequiredService<IWeekStatsSource>();
		//	await weekStatSource.FetchAndSaveAsync();
		//	weekStatSource.GetAll()
		//		.SelectMany(s => s.Players)
		//		.ToList()
		//		.ForEach(p => nflIdsToFetch.Add(p.NflId));

		//	var rosterSource = _serviceProvider.GetRequiredService<IRosterSource>();
		//	List<Roster> rosters = rosterSource.Get();
		//	rosters
		//		.SelectMany(r => r.Players)
		//		.ToList()
		//		.ForEach(p => nflIdsToFetch.Add(p.NflId));

		//	var playerProfileSource = _serviceProvider.GetRequiredService<IPlayerProfileSource>();
		//	await playerProfileSource.FetchAndSaveAsync(nflIdsToFetch.ToList());
		//}

		//private static async Task UpdateAllSourceFilesAsync()
		//{
		//	var weekStatsSource = _serviceProvider.GetRequiredService<IWeekStatsSource>();
		//	await weekStatsSource.FetchAndSaveAsync();
		//	var ids = weekStatsSource.GetAll()
		//		.SelectMany(s => s.Players)
		//		.Select(p => p.NflId)
		//		.ToList();


		//	var profileSource = _serviceProvider.GetRequiredService<IPlayerProfileSource>();
		//	await profileSource.FetchAndSaveAsync(ids);
		//}












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
