using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.PlayerData.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.PlayerData
{
	public interface IPlayerDataService
	{
		Core.Models.PlayerData GetPlayerData(string nflId);
		List<Core.Models.PlayerData> GetPlayerData(List<string> nflIds);
	}

	public class PlayerDataService : IPlayerDataService
	{
		private ILogger<PlayerDataService> _logger { get; }
		private FileDownloadConfig _fileDownloadConfig { get; }
		private IWebRequestClient _webRequestClient { get; }

		public PlayerDataService(
			ILogger<PlayerDataService> logger,
			FileDownloadConfig fileDownloadConfig,
			IWebRequestClient webRequestClient)
		{
			_logger = logger;
			_fileDownloadConfig = fileDownloadConfig;
			_webRequestClient = webRequestClient;
		}

		public Core.Models.PlayerData GetPlayerData(string nflId)
		{
			string path = _fileDownloadConfig.PlayerData + $"{nflId}.json";
			PlayerDataJson playerData = JsonConvert.DeserializeObject<PlayerDataJson>(File.ReadAllText(path));
			return PlayerDataJson.ToCoreEntity(playerData);
		}

		public List<Core.Models.PlayerData> GetPlayerData(List<string> nflIds)
		{
			return nflIds
				.Select(id => GetPlayerData(id))
				.ToList();
		}

		// ensures ALREADY EXISTING players ARENT fetched again
		public async Task SavePlayerDataFilesAsync(List<string> nflIds)
		{
			HashSet<string> existing = GetExistingPlayerNflIds();

			foreach (string id in nflIds)
			{
				if (existing.Contains(id))
				{
					continue;
				}

				NgsContentPlayer ngsContent = await GetNgsContentInfoAsync(id);
				NflPlayerProfile nflProfile = await GetNflPlayerProfileInfoAsync(id, ngsContent.FirstName, ngsContent.LastName);

				var playerData = new PlayerDataJson
				{
					NflId = ngsContent.NflId,
					FirstName = ngsContent.FirstName,
					LastName = ngsContent.LastName,
					//Position = ngsContent.Position,
					//TeamId = ngsContent.TeamId,
					//Number = nflProfile.Number,
					Height = nflProfile.Height,
					Weight = nflProfile.Weight,
					DateOfBirth = nflProfile.DateOfBirth.DateTime,
					College = nflProfile.College
				};

				string serializedPlayerData = JsonConvert.SerializeObject(playerData);

				string path = _fileDownloadConfig.PlayerData + $"{id}.json";
				File.WriteAllText(path, serializedPlayerData);

				existing.Add(id);
			}
		}

		private HashSet<string> GetExistingPlayerNflIds()
		{
			var directory = new DirectoryInfo(_fileDownloadConfig.PlayerData);

			return directory
				.GetFiles()
				.Select(f =>
				{
					string fileName = f.Name;
					var split = fileName.Split(".");
					return split[0];
				})
				.ToHashSet();
		}

		private async Task<NgsContentPlayer> GetNgsContentInfoAsync(string nflId)
		{
			string uri = $"http://api.fantasy.nfl.com/v2/player/ngs-content?playerId={nflId}";
			
			var response = await _webRequestClient.GetStringAsync(uri);
			var json = JsonConvert.DeserializeObject<NgsContentJson>(response);

			return NgsContentJson.ToEntity(json);
		}

		private async Task<NflPlayerProfile> GetNflPlayerProfileInfoAsync(string nflId, string firstName, string lastName)
		{
			string name = firstName.ToLower() + lastName.ToLower();
			string uri = $"http://www.nfl.com/player/{name}/{nflId}/profile";

			string html = await _webRequestClient.GetStringAsync(uri);

			var page = new HtmlDocument();
			page.LoadHtml(html);
			
			int number = PlayerProfileScraper.ExtractPlayerNumber(page);
			(int height, int weight) = PlayerProfileScraper.ExtractHeightWeight(page);
			DateTimeOffset dateOfBirth = PlayerProfileScraper.ExtractDateOfBirth(page);
			string college = PlayerProfileScraper.ExtractCollege(page);
			(string esbId, string gsisId) = PlayerProfileScraper.ExtractIds(page);
			string pictureUri = PlayerProfileScraper.ExtractPictureUri(page);

			return new NflPlayerProfile
			{
				EsbId = esbId,
				GsisId = gsisId,
				PictureUri = pictureUri,
				Number = number,
				Height = height,
				Weight = weight,
				DateOfBirth = dateOfBirth,
				College = college
			};
		}
	}
}
