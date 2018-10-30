using Newtonsoft.Json;
using R5.FFDB.Core.Services.Services;
using R5.FFDB.Sources.FantasyApi;
using R5.FFDB.Sources.FantasyApi.V2.Models;
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
			var config = new FantasyApiConfig();

			var fileSvc = new FileService(config);
			var fetcher = new FantasyApiService(config, fileSvc);

			fetcher.FetchAllAvailableToDiskAsync().GetAwaiter().GetResult();

			Console.ReadKey();
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
