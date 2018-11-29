using DevTester.Testers;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
		private static IServiceProvider _serviceProvider { get; set; }
		private static ILogger<DevProgram> _logger { get; set; }

		public static async Task Main(string[] args)
		{
			_serviceProvider = DevTestServiceProvider.Build();
			_logger = _serviceProvider.GetRequiredService<ILogger<DevProgram>>();

			await FetchPlayerProfilesFromRostersAsync(downloadRosterPages: false);

			// temp: fix roster (get first and last names)
			//var dataPath = _serviceProvider.GetRequiredService<DataDirectoryPath>();
			//var team = Teams.Get().First();
			//var rosterPagePath = dataPath.RosterPages + $"{team.Abbreviation}.html";
			//var pageHtml = File.ReadAllText(rosterPagePath);
			//var page = new HtmlDocument();
			//page.LoadHtml(pageHtml);
			//List<RosterPlayer> players = RosterScraper.ExtractPlayers(page)
			//	.Select(p => new RosterPlayer
			//	{
			//		NflId = p.nflId,
			//		Number = p.number,
			//		Position = p.position,
			//		Status = p.status
			//	})
			//	.ToList();

			Console.ReadKey();
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
