using HtmlAgilityPack;
using Newtonsoft.Json;
using R5.FFDB.Components.Roster;
using R5.FFDB.Components.Roster.Sources.NFLWebTeam;
using R5.FFDB.Components.WeekStats.Sources.NFLFantasyApi.Models;
using R5.FFDB.Core.Data;
using R5.FFDB.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Console;

namespace R5.FFDB.CLI
{
	class Program
	{
		static void Main(string[] args)
		{
			//var rosterSvc = new RosterSource(null, null);
			//var seahawks = Teams.Get().Single(t => t.Id == 30);
			//var roster = rosterSvc.GetForTeamAsync(seahawks).GetAwaiter().GetResult();

			//var playersWithoutNumbers = roster.Players.Where(p => !p.Number.HasValue).ToList();

			//return;

			//
			var setup = new EngineSetup();

			setup.WebRequest
				.AddDefaultBrowserHeaders()
				.SetRandomizedThrottle(3000, 8000);

			setup.FileDownload
				.SetWeekStatsDirectory(@"D:\Repos\ffdb_stuff\weekstat_files")
				.SetPlayerDataDirectory(@"D:\Repos\ffdb_stuff\playerdata_files");

			setup.Logging
				.SetLogDirectory(@"D:\Repos\ffdb_stuff\logs");

			var engine = setup.Create();

			engine.TestLogging();
			
			
			return;
		}




































		static async Task GetRavensPageSaveToDisk()
		{
			var url = "http://www.nfl.com/player/dougbaldwin/2530747/profile";
			var web = new HtmlWeb();
			HtmlDocument doc = web.Load(url);
			doc.Save(@"D:\Repos\ffdb_scrape_files\doug_baldwin_NFL.COM.html");
		}

		static async Task Test()
		{
			string endpoint = "https://api.nfl.com/v1/currentWeek";
			string downloadPath = @"D:\Repos\ffdb_scrape_files\";

			using (var client = new HttpClient())
			using (HttpResponseMessage response = await client.GetAsync(endpoint))
			using (HttpContent content = response.Content)
			{
				string result = await content.ReadAsStringAsync();

				//string fileWritePath = downloadPath += $"test.json";
				//System.IO.File.WriteAllText(fileWritePath, result);
			}
		}

		static async Task<WeekStatsJson> TestFantasyApiWeekStatsAsync()
		{
			string endpoint = "http://api.fantasy.nfl.com/v2/players/weekstats?season=2018&week=7";
			string downloadPath = @"D:\Repos\ffdb_weekstat_downloads\";

			try
			{
				using (var client = new HttpClient())
				using (HttpResponseMessage response = await client.GetAsync(endpoint))
				using (HttpContent content = response.Content)
				{
					string result = await content.ReadAsStringAsync();
					
					string fileWritePath = downloadPath += $"2018-7.json";
					System.IO.File.WriteAllText(fileWritePath, result);

					return JsonConvert.DeserializeObject<WeekStatsJson>(result);
				}
			}
			catch (Exception ex)
			{

				string ohNoes = "uh oh";
				return null;
			}
		}
	}
}
