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
using R5.FFDB.DbProviders.PostgreSql.DatabaseProvider;
using R5.FFDB.DbProviders.PostgreSql.Models;
using R5.FFDB.DbProviders.PostgreSql.Models.Entities;
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
			var dataPath = _serviceProvider.GetRequiredService<DataDirectoryPath>();

			//
			//var playerSource = _serviceProvider.GetRequiredService<IPlayerProfileSource>();
			//List<PlayerProfile> players = playerSource.Get();

			//var invalidPlayers = new List<PlayerProfile>();
			//foreach (var p in players)
			//{
			//	if (p.FirstName.Contains('#')
			//		|| p.LastName.Contains('#')
			//		|| p.College.Contains('#'))
			//	{
			//		invalidPlayers.Add(p);
			//	}
			//}

			//Console.WriteLine($"INVALID nflids: ");
			//foreach (var invalidP in invalidPlayers)
			//{
			//	Console.WriteLine(invalidP.NflId);
			//	File.Delete(dataPath.Static.PlayerProfile + invalidP.NflId + ".json");
			//}
			//
			
			var dbProvider = _serviceProvider.GetRequiredService<IDatabaseProvider>();
			IDatabaseContext dbContext = dbProvider.GetContext();

			await InitialSetupTestAsync();
			//await dbContext.TestInsertWithParamsAsync();

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

		// TODO: this will all be copied to the ENGINEs initial setup method
		//       after done and tested
		private static async Task InitialSetupTestAsync()
		{
			_logger.LogInformation("Running initial setup..");

			var dbProvider = _serviceProvider.GetRequiredService<IDatabaseProvider>();
			IDatabaseContext dbContext = dbProvider.GetContext();
			_logger.LogInformation($"Will run using database provider '{dbProvider.GetType().Name}'.");

			//await dbContext.CreateTablesAsync();
			//await dbContext.Team.AddTeamsAsync();

			var sourcesResolver = _serviceProvider.GetRequiredService<SourcesResolver>();
			Sources sources = await sourcesResolver.GetAsync();

			//await sources.Roster.FetchAndSaveAsync();
			//List<Roster> rosters = sources.Roster.Get();

			//await sources.WeekStats.FetchAndSaveAsync();
			List<WeekStats> weekStats = sources.WeekStats.GetAll();

			_logger.LogInformation("Fetching player profiles for players resolved from roster and week stats.");

			//List<string> playerNflIds = rosters
			//	.SelectMany(r => r.Players)
			//	.Select(p => p.NflId)
			//	.Concat(weekStats.SelectMany(ws => ws.Players).Select(p => p.NflId))
			//	.ToList();

			//await sources.PlayerProfile.FetchAndSaveAsync(playerNflIds);

			_logger.LogInformation("Beginning persisting of player profiles to database..");

			//List<PlayerProfile> players = sources.PlayerProfile.Get();
			//await dbContext.Player.AddAsync(players, rosters);

			_logger.LogInformation("Beginning persisting of player-team mappings to database..");

			// RESEARCH: should we have entries for all players, with potentially null TEAM id values?
			//    OR only include entries for players CURRENTLY on a team?

			//await dbContext.Team.UpdateRostersAsync(rosters);

			// TESTING: week stat adds
			List<WeekStats> orderedWeeks = weekStats.OrderBy(ws => ws.Week).ToList();
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
			List<Roster> rosters = rosterSource.Get();
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
