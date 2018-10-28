using Newtonsoft.Json;
using R5.FFDB.Sources.FantasyApi;
using R5.FFDB.Sources.FantasyApi.V2.RequestModels;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Console;

namespace R5.FFDB.CLI
{
	class Program
	{
		static void Main(string[] args)
		{

			//WeekStats weekStats = TestFantasyApiWeekStatsAsync().GetAwaiter().GetResult();

			//var fetcher = new WeekStatsFetcher(null);
			//int latestWeek = fetcher.GetLatestCompletedWeekAsync().GetAwaiter().GetResult();

			var fileSvc = new FileService(null);
			System.Collections.Generic.List<Core.Abstractions.WeekInfo> result = fileSvc.GetExistingWeeks(new Core.Abstractions.WeekInfo(1, 1));

			Console.ReadKey();
		}

		static async Task<WeekStats> TestFantasyApiWeekStatsAsync()
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

					return JsonConvert.DeserializeObject<WeekStats>(result);
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
