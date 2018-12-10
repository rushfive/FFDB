using DevTester.Testers;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R5.FFDB.Components.PlayerProfile;
using R5.FFDB.Components.PlayerTeamHistory;
using R5.FFDB.Components.PlayerTeamHistory.Sources.NFLWeb;
using R5.FFDB.Components.PlayerTeamHistory.Sources.NFLWeb.Models;
using R5.FFDB.Database;
using R5.FFDB.DbProviders.PostgreSql;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

			await FetchAllPlayerTeamHistoriesAsync();

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

		private static async Task FetchAllPlayerTeamHistoriesAsync()
		{
			IPlayerProfileSource profileSource = _serviceProvider.GetRequiredService<IPlayerProfileSource>();
			IPlayerTeamHistorySource historySource = _serviceProvider.GetRequiredService<IPlayerTeamHistorySource>();

			List<R5.FFDB.Core.Models.PlayerProfile> allPlayers = profileSource.GetAll();
			await historySource.FetchAndSaveAsync(allPlayers);
		}

		private static Task FetchPlayerTeamHistoryAsync(string nflId, string firstName, string lastName)
		{
			try
			{
				IPlayerTeamHistorySource source = _serviceProvider.GetRequiredService<IPlayerTeamHistorySource>();

				return source.FetchAndSaveAsync(new List<R5.FFDB.Core.Models.PlayerProfile>
				{
					new R5.FFDB.Core.Models.PlayerProfile
					{
						NflId = nflId,
						FirstName = firstName,
						LastName = lastName
					}
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "There was an error fetching player team history.");
				throw;
			}
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
