using Newtonsoft.Json.Linq;
using R5.FFDB.Core.Abstractions;
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
		private FileService _fileService { get; }

		public WeekStatsFetcher(
			FantasyApiSourceConfig config,
			FileService fileService)
		{
			_config = config;
			_fileService = fileService;
		}

		// lot more todos: parallel requests? making it non-blocking if updating CLIs ui with progress?
		public async Task FetchAllAvailableToDiskAsync()
		{
			// - get latest completed weeks
			// - now we know the range of total possible weeks to fetch (2010-1 to latest)
			// - scan the download directory, and find the diff of what's missing (throw if invalid file found via regex??)
			// - fetch the diff'd weeks and save to disk!
			WeekInfo latestCompleted = await GetLatestCompletedWeekAsync();

			List<WeekInfo> missingWeeks = _fileService.GetMissingWeeks(latestCompleted);

			foreach(WeekInfo week in missingWeeks)
			{
				string endpoint = FantasyApiEndpoint.V2.WeekStatsUrl(week.Season, week.Week);
				string weekStats = await Http.Request.GetAsStringAsync(endpoint);

				_fileService.SaveWeekStatsToDisk(weekStats, week);

				await Task.Delay(_config.RequestDelayMilliseconds);
			}
		}

		// todo: private
		public async Task<WeekInfo> GetLatestCompletedWeekAsync()
		{
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

					(int currentSeason, int currentWeek) = GetCurrentWeekInfo(weekStats);

					return new WeekInfo(currentSeason, currentWeek);
				}
			}
			catch (Exception ex)
			{
				// todo
				throw;
			}
		}

		// pass the entire FantasyApi WeekStats response, parsed into a JObject
		private (int currentSeason, int currentWeek) GetCurrentWeekInfo(JObject weekStats)
		{
			JObject games = weekStats["games"].ToObject<JObject>();

			string gameId = games.Properties().Select(p => p.Name).First();

			int season = games[gameId]["season"].ToObject<int>();
			int currentWeek = games[gameId]["state"]["week"].ToObject<int>();

			bool isCompleted = games[gameId]["state"]["isWeekGamesCompleted"].ToObject<bool>();
			if (!isCompleted)
			{
				currentWeek = currentWeek - 1;
			}

			return (season, currentWeek);
		}

		// todo: asynchrnous/non-blocking
		//private List<(int Season, int Week)> ResolveMissingWeeks()
		//{

		//}

	}
}
