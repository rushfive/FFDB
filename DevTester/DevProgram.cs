using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using R5.FFDB.Components;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.Roster.Sources.NFLWebTeam;
using R5.FFDB.Core.Data;
using R5.FFDB.Core.Models;
using R5.FFDB.Core.Sources;
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
		static DataDirectoryPath DataPath = new DataDirectoryPath(@"D:\Repos\ffdb_data\");
		static WebRequestClient WebClient = GetWebRequestClient();

		public static async Task Main(string[] args)
		{
			//DownloadRosterPagesAsync()
			//	.ConfigureAwait(false)
			//	.GetAwaiter()
			//	.GetResult();

			var rosters = GetRosters();

			Console.ReadKey();
		}

		public static IServiceProvider BuildDevTestServiceProvider()
		{
			var services = new ServiceCollection();

			WebRequestClient webClient = GetWebRequestClient();

			services
				.AddScoped<IWebRequestClient>(sp => webClient);

			AddLogging(services, @"D:\Repos\ffdb_data\dev_test_logs\");

			return services.BuildServiceProvider();

			// local funcs
			void AddLogging(ServiceCollection sc, string logDirectory)
			{
				Log.Logger = new LoggerConfiguration()
					.MinimumLevel.Debug()
					.WriteTo.Console()
					.WriteTo.File(
						logDirectory + ".txt",
						fileSizeLimitBytes: null,
						restrictedToMinimumLevel: LogEventLevel.Debug,
						rollingInterval: RollingInterval.Hour,
						rollOnFileSizeLimit: false)
					.CreateLogger();

				sc.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
			}
		}

		public static List<Roster> GetRosters()
		{
			var rosters = new List<Roster>();

			List<Team> teams = Teams.Get();//.GetRange(0, 1);

			foreach(var team in teams)
			{
				string pagePath = DataPath.RosterPages + $"{team.Abbreviation}.html";
				var pageHtml = File.ReadAllText(pagePath);

				var page = new HtmlDocument();
				page.LoadHtml(pageHtml);

				List<RosterPlayer> players = RosterScraper.ExtractPlayers(page)
					.Select(p => new RosterPlayer
					{
						NflId = p.nflId,
						Number = p.number,
						Position = p.position,
						Status = p.status
					})
					.ToList();

				rosters.Add(new Roster
				{
					TeamId = team.Id,
					TeamAbbreviation = team.Abbreviation,
					Players = players
				});
			}

			return rosters;
		}

		public static async Task DownloadRosterPagesAsync()
		{
			List<Team> teams = Teams.Get();

			// temp: get first
			//var team = teams.First();
			//string page = await WebClient.GetStringAsync(team.RosterSourceUris[RosterSourceKeys.NFLWebTeam], throttle: true);
			//await File.WriteAllTextAsync(DataPath.RosterPages + $"__TEST__{team.Abbreviation}.html", page);

			foreach (Team team in teams.Where(t => t.RosterSourceUris.ContainsKey(RosterSourceKeys.NFLWebTeam)))
			{
				string page = await WebClient.GetStringAsync(team.RosterSourceUris[RosterSourceKeys.NFLWebTeam], throttle: true);
				await File.WriteAllTextAsync(DataPath.RosterPages + $"{team.Abbreviation}.html", page);
			}
		}

		public static WebRequestClient GetWebRequestClient()
		{
			var headers = new Dictionary<string, string>
			{
				//{ "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8" },
				//{ "Accept-Encoding", "gzip, deflate" },
				//{ "Accept-Language", "en-US,en;q=0.9" },
				{ "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36" }
			};

			var config = new WebRequestConfig(0, (3000, 8000), headers);
			return new WebRequestClient(config);
		}
	}
}
