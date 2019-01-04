using Newtonsoft.Json;
using R5.FFDB.Components.CoreData.PlayerProfile.Models;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace R5.FFDB.Components.CoreData.PlayerProfile.Values
{
	public class PlayerProfilesValue : ValueProvider<List<Core.Models.PlayerProfile>>
	{
		private DataDirectoryPath _dataPath { get; }

		public PlayerProfilesValue(DataDirectoryPath dataPath)
			: base("Player Profiles")
		{
			_dataPath = dataPath;
		}

		protected override List<Core.Models.PlayerProfile> ResolveValue()
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
