using Newtonsoft.Json;
using R5.FFDB.Components.CoreData.Players.Models;
using R5.FFDB.Components.Resolvers;
using R5.FFDB.Core.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace R5.FFDB.Components.CoreData.Players
{
	public interface IPlayerService
	{
		List<Player> Get(List<string> nflIds);
	}

	public class PlayerService : IPlayerService
	{
		private DataDirectoryPath _dataPath { get; }

		public PlayerService(
			DataDirectoryPath dataPath)
		{
			_dataPath = dataPath;
		}

		public List<Player> Get(List<string> nflIds)
		{
			// file names are formatted as {nflId}.json
			var files = DirectoryFilesResolver.GetFileNames(_dataPath.Temp.Player, excludeExtensions: true);

			return files
				.Where(f => nflIds.Contains(f))
				.Select(f =>
				{
					string filePath = _dataPath.Temp.Player + $"{f}.json";
					PlayerJson json = JsonConvert.DeserializeObject<PlayerJson>(File.ReadAllText(filePath));
					return PlayerJson.ToCoreEntity(json);
				})
				.ToList();
		}
	}
}
