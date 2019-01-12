using HtmlAgilityPack;
using Newtonsoft.Json;
using R5.FFDB.Components.CoreData.WeekStats.Models;
using R5.FFDB.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Console;

namespace R5.FFDB.CLI
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			//await FetchGameCenterDataAsync();


			XElement gcXml = XElement.Load(@"D:\Repos\ffdb_data\temp\gc.xml");
			
			List<XElement> gameNodes = gcXml.Elements("gms").Single().Elements("g").ToList();
			List<string> gameIds = gameNodes.Select(g => g.Attribute("eid").Value).ToList();


			//Console.WriteLine(gcXml);

			//var url = @"http://www.nfl.com/liveupdate/game-center/2012020500/2012020500_gtd.json";
			//var client = new HttpClient();
			//string r = await client.GetStringAsync(url);
			//System.IO.File.WriteAllText(@"D:\Repos\ffdb_data\temp\2012020500_gtd.json", r);


			return;









			//var rosterSvc = new RosterSource(null, null);
			//var seahawks = Teams.Get().Single(t => t.Id == 30);
			//var roster = rosterSvc.GetForTeamAsync(seahawks).GetAwaiter().GetResult();

			//var playersWithoutNumbers = roster.Players.Where(p => !p.Number.HasValue).ToList();

			//return;
			//var client = new HttpClient();
			//string jdUri = @"http://www.nfl.com/player/j.d.mckissic/2556440/profile";
			//string jdPage = await client.GetStringAsync(jdUri);

			//var page = new HtmlDocument();
			//page.LoadHtml(jdPage);

			//(string firstName, string lastName) = PlayerProfileScraper.ExtractNames(page);


			//
			var setup = new EngineSetup();

			setup.WebRequest
				.AddDefaultBrowserHeaders()
				.SetThrottle(2500);
				//.SetRandomizedThrottle(3000, 8000);

			setup.SetRootDataDirectoryPath(@"D:\Repos\ffdb_data\");
			//setup.FileDownload
			//	.SetWeekStatsDirectory(@"D:\Repos\ffdb_stuff\weekstat_files")
			//	.SetPlayerDataDirectory(@"D:\Repos\ffdb_stuff\playerdata_files");

			setup.Logging
				.SetLogDirectory(@"D:\Repos\ffdb_data\logs");

			FfdbEngine engine = setup.Create();

			await engine.RunInitialSetupAsync(false);
			
			return;
		}










		private static async Task FetchGameCenterDataAsync()
		{
			var url = "http://www.nfl.com/ajax/scorestrip?season=2018&seasonType=REG&week=1";
			var client = new HttpClient();
			string r = await client.GetStringAsync(url);
			System.IO.File.WriteAllText(@"D:\Repos\ffdb_data\temp\gc.xml", r);










			return;






			// http://www.nfl.com/ajax/scorestrip?season=2018&seasonType=REG&week=1

			var web = new HtmlWeb();
			HtmlDocument doc = web.Load(url);
			doc.Save(@"D:\Repos\ffdb_data\temp\gc.xml");
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
