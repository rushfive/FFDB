using HtmlAgilityPack;
using Newtonsoft.Json;
using R5.FFDB.Core.Abstractions;
using R5.FFDB.Core.Components;
using R5.FFDB.Core.Components.Roster;
using R5.FFDB.Core.Components.Setup.Services;
using R5.FFDB.Core.Components.WeekStats.Models;
using R5.FFDB.Core.Data;
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
			var config = new FfdbConfig();
			var hawksTeam = Teams.Get().Single(t => t.Id == 30);

			var rosterSvc = new RosterService(config);

			Core.Game.Roster roster = rosterSvc.GetForTeamAsync(hawksTeam).GetAwaiter().GetResult();


			//var initialSetupSvc = new InitialSetupService();

			//var playerIds = new List<string>
			//{
			//	"2530747", //dougb
			//	"2558125" //mahomes
			//};

			//initialSetupSvc.SavePlayerDataFilesAsync(playerIds)
			//	.GetAwaiter()
			//	.GetResult();

			//var playerData = initialSetupSvc.GetPlayerDataJson(playerIds[0]);


			return;

			
			return;
			/// depth charts by team:
			/// http://feeds.nfl.com/feeds-rs/depthChartClub/byTeam/SEA.json


			var doc = new HtmlDocument();
			doc.Load(@"D:\Repos\ffdb_scrape_files\seahawks_roster_NFL.com.html");

			var body = doc.DocumentNode.SelectSingleNode("//body");
//			var t = doc.DocumentNode.SelectSingleNode("//table");


			//var resultTable = doc.DocumentNode.SelectSingleNode("//@id=result");
			HtmlNode result = doc.GetElementbyId("result");

			HtmlNodeCollection rows = doc.GetElementbyId("result")
				?.SelectSingleNode("//tbody")
				?.SelectNodes("tr");

			foreach(HtmlNode r in rows)
			{
				getPlayerInfoFromRow(r);
			}

			//GetRavensPageSaveToDisk().GetAwaiter().GetResult();

			Console.ReadKey();

			// local funcs
			(int nflId, string firstName, string lastName) getPlayerInfoFromRow(HtmlNode row)
			{
				var cells = row.SelectNodes("td");
				var targetCell = cells[1];

				//HtmlNode a = targetCell.ChildNodes.First(n => n.InnerText != " ");

				////var a2 = targetCell
				//var first = targetCell.ChildNodes[0];
				//var second = targetCell.ChildNodes[1];//a

				var anchor = targetCell.ChildNodes.Single(n => n.NodeType == HtmlNodeType.Element);

				//targetCell.ChildNodes.Single(n => n.Attributes["href"] != null)
//				HtmlNode anchor = row.ChildNodes[1].FirstChild;

				string profileUri = anchor.Attributes["href"].Value;
				string name = anchor.InnerText;

				return (0, null, null);
			}
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
