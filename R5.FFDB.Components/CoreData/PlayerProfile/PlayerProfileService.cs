using Newtonsoft.Json;
using R5.FFDB.Components.CoreData.PlayerProfile.Models;
using R5.FFDB.Components.Resolvers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace R5.FFDB.Components.CoreData.PlayerProfile
{
	public interface IPlayerProfileService
	{
		List<Core.Models.PlayerProfile> Get(List<string> nflIds);
		List<Core.Models.PlayerProfile> Get();
	}

	public class PlayerProfileService : IPlayerProfileService
	{
		private DataDirectoryPath _dataPath { get; }

		public PlayerProfileService(
			DataDirectoryPath dataPath)
		{
			_dataPath = dataPath;
		}

		public List<Core.Models.PlayerProfile> Get(List<string> nflIds)
		{
			// file names are formatted as {nflId}.json
			var files = DirectoryFilesResolver.GetFileNames(_dataPath.Temp.PlayerProfile, excludeExtensions: true);

			return files
				.Where(f => nflIds.Contains(f))
				.Select(f =>
				{
					string filePath = _dataPath.Temp.PlayerProfile + $"{f}.json";
					PlayerProfileJson json = JsonConvert.DeserializeObject<PlayerProfileJson>(File.ReadAllText(filePath));
					return PlayerProfileJson.ToCoreEntity(json);
				})
				.ToList();
		}

		// pre per-week below

		public List<Core.Models.PlayerProfile> Get()
		{
			var directory = new DirectoryInfo(_dataPath.Temp.PlayerProfile);

			return directory
				.GetFiles()
				.Select(f =>
				{
					string filePath = f.ToString();
					PlayerProfileJson json = JsonConvert.DeserializeObject<PlayerProfileJson>(File.ReadAllText(filePath));
					return PlayerProfileJson.ToCoreEntity(json);
				})
				.ToList();
		}
	}
}
