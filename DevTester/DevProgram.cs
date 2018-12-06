using DevTester.Testers;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R5.FFDB.Components.PlayerTeamHistory;
using R5.FFDB.Components.PlayerTeamHistory.Sources.NFLWeb;
using R5.FFDB.Components.PlayerTeamHistory.Sources.NFLWeb.Models;
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
			//var url = "http://www.nfl.com/player/chriscarson/2558865/profile";
			//var web = new HtmlWeb();
			//HtmlDocument doc = web.Load(url);
			//doc.Save(@"D:\Repos\ffdb_data\temp\chris_carson_profile.html");

			string pagePath = @"D:\Repos\ffdb_data\temp\chris_carson_profile.html";
			var pageHtml = File.ReadAllText(pagePath);
			var page = new HtmlDocument();
			page.LoadHtml(pageHtml);

			var seasons = PlayerTeamHistoryScraper.ExtractSeasonsPlayed(page);

			///////////////

			//_serviceProvider = DevTestServiceProvider.Build();
			//_logger = _serviceProvider.GetRequiredService<ILogger<DevProgram>>();

			//await FetchPlayerTeamHistoryAsync("NFL_ID", "Chris", "Carson");

			///////////////

			//var postgresConfig = new PostgresConfig
			//{
			//	DatabaseName = "ffdb_test_1",
			//	Host = "localhost",
			//	Username = "ffdb",
			//	Password = "welc0me!"
			//};

			//var postgresProvider = new PostgresDbProvider(postgresConfig);
			//PostgresDbContext context = postgresProvider.GetContext();

			//await context.RunInitialSetupAsync();

			//////////

			//_serviceProvider = DevTestServiceProvider.Build();
			//_logger = _serviceProvider.GetRequiredService<ILogger<DevProgram>>();

			//await FetchPlayerProfilesFromRostersAsync(downloadRosterPages: false);


			Console.ReadKey();
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
	}
}
