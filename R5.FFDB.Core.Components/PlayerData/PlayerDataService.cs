using HtmlAgilityPack;
using Newtonsoft.Json;
using R5.FFDB.Core.Components.PlayerData.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Core.Components.PlayerData
{
	public class PlayerDataService
	{
		private FfdbConfig _config { get; }

		public PlayerDataService(FfdbConfig config)
		{
			_config = config;
		}

		public Core.Game.PlayerData GetPlayerData(string nflId)
		{
			string path = _config.PlayerDataPath + $"{nflId}.json";
			PlayerDataJson playerData = JsonConvert.DeserializeObject<PlayerDataJson>(File.ReadAllText(path));
			return PlayerDataJson.ToCoreEntity(playerData);
		}

		public List<Core.Game.PlayerData> GetPlayerData(List<string> nflIds)
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
					Position = ngsContent.Position,
					TeamId = ngsContent.TeamId,
					Number = nflProfile.Number,
					Height = nflProfile.Height,
					Weight = nflProfile.Weight,
					DateOfBirth = nflProfile.DateOfBirth.DateTime,
					College = nflProfile.College
				};

				string serializedPlayerData = JsonConvert.SerializeObject(playerData);

				string path = _config.PlayerDataPath + $"{id}.json";
				File.WriteAllText(path, serializedPlayerData);

				existing.Add(id);

				await Task.Delay(_config.RequestDelayMilliseconds);
			}
		}

		private HashSet<string> GetExistingPlayerNflIds()
		{
			var directory = new DirectoryInfo(_config.PlayerDataPath);

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
			
			var response = await Http.Client.GetStringAsync(uri);
			var json = JsonConvert.DeserializeObject<NgsContentJsonV2>(response);

			return NgsContentJsonV2.ToEntity(json);
		}

		private async Task<NflPlayerProfile> GetNflPlayerProfileInfoAsync(string nflId, string firstName, string lastName)
		{
			string name = firstName.ToLower() + lastName.ToLower();
			string uri = $"http://www.nfl.com/player/{name}/{nflId}/profile";

			var web = new HtmlWeb();
			HtmlDocument page = web.Load(uri);

			int number = PlayerProfileScraper.ExtractPlayerNumber(page);
			(int height, int weight) = PlayerProfileScraper.ExtractHeightWeight(page);
			DateTimeOffset dateOfBirth = PlayerProfileScraper.ExtractDateOfBirth(page);
			string college = PlayerProfileScraper.ExtractCollege(page);

			return new NflPlayerProfile
			{
				Number = number,
				Height = height,
				Weight = weight,
				DateOfBirth = dateOfBirth,
				College = college
			};
		}
	}
}
