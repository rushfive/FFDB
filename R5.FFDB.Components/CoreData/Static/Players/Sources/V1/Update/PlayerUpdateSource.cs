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
	public class PlayerUpdateSource : CoreDataSource<PlayerUpdateVersioned, Player, string>
	{
		public PlayerUpdateSource(
			ILogger<PlayerUpdateSource> logger,
			ToVersionedMapper toVersionedMapper,
			ToCoreMapper toCoreMapper,
			ProgramOptions programOptions,
			IDatabaseProvider dbProvider,
			DataDirectoryPath dataPath,
			IWebRequestClient webClient)
			: base(
				  logger,
				  toVersionedMapper,
				  toCoreMapper,
				  programOptions,
				  dbProvider,
				  dataPath,
				  webClient)
		{ }

		protected override bool SupportsFilePersistence => false;

		protected override string GetVersionedFilePath(string nflId)
		{
			return DataPath.Player(nflId);
		}

		protected override string GetSourceUri(string nflId)
		{
			return Endpoints.Page.PlayerProfile(nflId);
		}

		protected override Task OnVersionedModelMappedAsync(string key, PlayerUpdateVersioned versioned)
		{
			return Task.CompletedTask;
		}

		protected override Task OnCoreDataMappedAsync(string key, Player coreData)
		{
			return Task.CompletedTask;
		}
	}
}
