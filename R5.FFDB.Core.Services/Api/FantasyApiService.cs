//using Newtonsoft.Json.Linq;
//using R5.FFDB.Core.Abstractions;
//using R5.FFDB.Core.Request;
//using R5.FFDB.Sources.FantasyApi;
//using R5.FFDB.Sources.FantasyApi.V2.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;

//namespace R5.FFDB.Core.Services.Api
//{
//	public class FantasyApiService
//	{
//		private FantasyApiConfig _config { get; }
//		private FileService _fileService { get; }

//		public FantasyApiService(
//			FantasyApiConfig config,
//			FileService fileService)
//		{
//			_config = config;
//			_fileService = fileService;
//		}

//		public FantasyApiWeekStats GetWeekStats(WeekInfo week)
//		{
//			WeekStatsJson statsJson = _fileService.GetWeekStats(week);
//			return WeekStatsJson.ToCoreEntity(statsJson);
//		}

//		// lot more todos: parallel requests? making it non-blocking if updating CLIs ui with progress?
//		public async Task FetchAllAvailableToDiskAsync()
//		{
//			WeekInfo latestCompleted = await GetLatestCompletedWeekAsync();

//			List<WeekInfo> missingWeeks = _fileService.GetMissingWeeks(latestCompleted);

//			foreach (WeekInfo week in missingWeeks)
//			{
//				string endpoint = FantasyApiEndpoint.V2.WeekStatsUrl(week.Season, week.Week);
//				string weekStats = await Http.Request.GetAsStringAsync(endpoint);

//				_fileService.SaveWeekStatsToDisk(weekStats, week);

//				await Task.Delay(_config.RequestDelayMilliseconds);
//			}
//		}
		
//		private async Task<WeekInfo> GetLatestCompletedWeekAsync()
//		{
//			// doesn't matter which week we choose, it'll always return
//			// the NFL's current state info
//			string endpoint = FantasyApiEndpoint.V2.WeekStatsUrl(2018, 1);

//			try
//			{
//				using (var client = new HttpClient())
//				using (HttpResponseMessage response = await client.GetAsync(endpoint))
//				using (HttpContent content = response.Content)
//				{
//					string weekStatsJson = await content.ReadAsStringAsync();

//					JObject weekStats = JObject.Parse(weekStatsJson);

//					(int currentSeason, int currentWeek) = GetCurrentWeekInfo(weekStats);

//					return new WeekInfo(currentSeason, currentWeek);
//				}
//			}
//			catch (Exception ex)
//			{
//				// todo
//				throw;
//			}
//		}

//		// pass the entire FantasyApi WeekStats response, parsed into a JObject
//		private (int currentSeason, int currentWeek) GetCurrentWeekInfo(JObject weekStats)
//		{
//			JObject games = weekStats["games"].ToObject<JObject>();

//			string gameId = games.Properties().Select(p => p.Name).First();

//			int season = games[gameId]["season"].ToObject<int>();
//			int currentWeek = games[gameId]["state"]["week"].ToObject<int>();

//			bool isCompleted = games[gameId]["state"]["isWeekGamesCompleted"].ToObject<bool>();
//			if (!isCompleted)
//			{
//				currentWeek = currentWeek - 1;
//			}

//			return (season, currentWeek);
//		}

//	}
//}
