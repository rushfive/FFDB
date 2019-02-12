using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Update.Mappers;
using R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Update.Models;
using R5.FFDB.Components.Http;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Update
{
	public class PlayerUpdateSource : CoreDataSource<PlayerUpdateVersioned, PlayerUpdate, string>
	{
		public PlayerUpdateSource(
			ILogger<PlayerUpdateSource> logger,
			IToVersionedMapper toVersionedMapper,
			IToCoreMapper toCoreMapper,
			ProgramOptions programOptions,
			DataDirectoryPath dataPath,
			IWebRequestClient webClient)
			: base(
				  logger,
				  toVersionedMapper,
				  toCoreMapper,
				  programOptions,
				  dataPath,
				  webClient)
		{ }

		protected override bool SupportsSourceFilePersistence => false;
		protected override bool SupportsVersionedFilePersistence => false;

		protected override string GetVersionedFilePath(string nflId)
		{
			return null;
		}

		protected override string GetSourceFilePath(string key)
		{
			return null;
		}

		protected override string GetSourceUri(string nflId)
		{
			return Endpoints.Page.PlayerProfile(nflId);
		}
	}
}
