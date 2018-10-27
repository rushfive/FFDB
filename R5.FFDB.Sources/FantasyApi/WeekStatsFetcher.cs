using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Sources.FantasyApi
{
	// todo move
	public static class FantasyApiEndpoint
	{
		public static class V2
		{
			public static string WeekStatsUrl(int season, int week)
				=> $"http://api.fantasy.nfl.com/v2/players/weekstats?season={season}&week={week}";
		}
	}

	public class WeekStatsFetcher
	{
		private FantasyApiSourceConfig _config { get; }


		public WeekStatsFetcher(FantasyApiSourceConfig config)
		{
			_config = config;
		}

		public async Task FetchAllAvailableAsync()
		{

		}

		// todo: private
		public async Task<int> GetLatestCompletedWeekAsync()
		{
			//string filePath = @"D:\Repos\ffdb_weekstat_downloads\2018-7.json";

			//JObject stats = JObject.Parse(File.ReadAllText(filePath));

			//var games = stats["games"].ToObject<JObject>();

			//string gameId = games.Properties().Select(p => p.Name).First();

			//int currentWeek = games[gameId]["state"]["week"].ToObject<int>();
			//bool weekCompleted = games[gameId]["state"]["isWeekGamesCompleted"].ToObject<bool>();

			//foreach (KeyValuePair<string, JToken> x in games)
			//{
			//	string name = x.Key;
			//	JToken gameInfo = x.Value;
			//}

			

			////string test = "Here";
			//return;
			// REAL BELOW, temp get from disk above!!!!!!

			// any week stats update returns info on current NFL "state", including
			// the current week and "isWeekGamesCompleted". If true, use that week. If false, use previous.

			string endpoint = FantasyApiEndpoint.V2.WeekStatsUrl(2018, 1);

			try
			{
				using (var client = new HttpClient())
				using (HttpResponseMessage response = await client.GetAsync(endpoint))
				using (HttpContent content = response.Content)
				{
					string weekStatsJson = await content.ReadAsStringAsync();

					JObject weekStats = JObject.Parse(weekStatsJson);

					(int currentWeek, bool isCompleted) = GetCurrentWeekInfo(weekStats);

					return isCompleted ? currentWeek : currentWeek - 1;
				}
			}
			catch (Exception ex)
			{
				// todo
				throw;
			}
		}

		// pass the entire FantasyApi WeekStats response, parsed into a JObject
		private (int currentWeek, bool isCompleted) GetCurrentWeekInfo(JObject weekStats)
		{
			JObject games = weekStats["games"].ToObject<JObject>();

			string gameId = games.Properties().Select(p => p.Name).First();

			int currentWeek = games[gameId]["state"]["week"].ToObject<int>();
			bool isCompleted = games[gameId]["state"]["isWeekGamesCompleted"].ToObject<bool>();

			return (currentWeek, isCompleted);
		}

		// todo: asynchrnous/non-blocking
		//private List<(int Season, int Week)> ResolveMissingWeeks()
		//{

		//}

	}
}
