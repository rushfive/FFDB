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
using R5.FFDB.Components.CoreData.Roster.Values;
using R5.FFDB.Components.CoreData.TeamData.Models;
using R5.FFDB.Components.CoreData.TeamGameHistory;
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
	public class Test
	{
		public IDatabaseProvider DbProvider { get; }

		public Test(IDatabaseProvider dbProvider)
		{
			DbProvider = dbProvider;
		}
	}
	public class DevProgram
	{
		private static IServiceProvider _serviceProvider { get; set; }
		private static ILogger<DevProgram> _logger { get; set; }
		private static IDatabaseContext _dbContext { get; set; }

		public static async Task Main(string[] args)
		{
			_serviceProvider = DevTestServiceProvider.Build();
			_logger = _serviceProvider.GetService<ILogger<DevProgram>>();
			var dbProvider = _serviceProvider.GetRequiredService<IDatabaseProvider>();
			_dbContext = dbProvider.GetContext();
			/// DONT TOUCH ABOVE ///
			/// 


			FfdbEngine engine = GetConfiguredEngine();
			//await engine.Update.UpdateRostersAsync();
			await engine.Update.UpdateStatsForWeekAsync(new WeekInfo(2010, 2));
		
			

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
		private static async Task TestUpdateWeekAsync(WeekInfo week)
		{
			var wks = new List<WeekInfo> { week };

			var dbProvider = _serviceProvider.GetRequiredService<IDatabaseProvider>();
			IDatabaseContext dbContext = dbProvider.GetContext();
			var tgHistorySource = _serviceProvider.GetRequiredService<ITeamGameHistorySource>();
			var tgStatsSvc = _serviceProvider.GetRequiredService<ITeamGameStatsService>();
			var wsSource = _serviceProvider.GetRequiredService<IWeekStatsSource>();
			var wsSvc = _serviceProvider.GetRequiredService<IWeekStatsService>();
			var pSource = _serviceProvider.GetRequiredService<IPlayerProfileSource>();
			var pSvc = _serviceProvider.GetRequiredService<IPlayerProfileService>();
			//var rSource = _serviceProvider.GetRequiredService<IRosterSource>();
			//var rSvc = _serviceProvider.GetRequiredService<IRosterService>();

			// need overwrite option with a check first on update_log

			await tgHistorySource.FetchForWeeksAsync(wks);
			await wsSource.FetchForWeeksAsync(wks);

			var rostersValue = _serviceProvider.GetRequiredService<RostersValue>();
			List<Roster> rosters = await rostersValue.GetAsync();

			// Add player profiles

			//List<string> rosterNflIds = rosters.SelectMany(r => r.Players).Select(p => p.NflId).ToList();
			//await AddNewPlayersAsync(rosterNflIds);

			List<string> weekStatNflIds = wsSvc.GetNflIdsForWeek(week);
			await AddNewPlayersAsync(weekStatNflIds);

			// Add week stats
			WeekStats weekStats = await wsSvc.GetForWeekAsync(week);
			await _dbContext.Stats.UpdateWeekAsync(weekStats);

			// team stats
			List<TeamWeekStats> teamStats = tgStatsSvc.GetForWeek(week);
			await _dbContext.Team.UpdateGameStatsAsync(teamStats);

			await _dbContext.AddUpdateLogAsync(week);

			var t = "test";
		}

		private static async Task AddNewPlayersAsync(List<string> nflIds)
		{
			var profileSource = _serviceProvider.GetRequiredService<IPlayerProfileSource>();
			var profileService = _serviceProvider.GetRequiredService<IPlayerProfileService>();
			var rostersValue = _serviceProvider.GetRequiredService<RostersValue>();

			List<PlayerProfile> existing = await _dbContext.Player.GetAllAsync();
			HashSet<string> existingIds = existing.Select(p => p.NflId).ToHashSet();

			List<string> newIds = nflIds.Where(id => !existingIds.Contains(id)).ToList();
			await profileSource.FetchAsync(newIds);

			List<PlayerProfile> playerProfiles = profileService.Get(newIds);
			if (!playerProfiles.Any())
			{
				_logger.LogInformation("No new player profiles to add.");
				return;
			}

			List<Roster> rosters = await rostersValue.GetAsync();
			await _dbContext.Player.UpdateAsync(playerProfiles, rosters);
		}

		
		private static FfdbEngine GetConfiguredEngine()
		{
			var setup = new EngineSetup();

			setup
				.SetRootDataDirectoryPath(@"D:\Repos\ffdb_data_2\")
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

			return setup.Create();
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
