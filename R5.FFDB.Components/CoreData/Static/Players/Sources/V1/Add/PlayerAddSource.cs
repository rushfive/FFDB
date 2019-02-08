using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1;
using R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Add.Mappers;
using R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Add.Models;
using R5.FFDB.Components.Http;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.Players.Sources.V1.Add
{
	public class PlayerAddSource : CoreDataSource<PlayerAddVersionedModel, PlayerAdd, string>
	{
		private IRosterCache _rosterCache { get; }

		public PlayerAddSource(
			IRosterCache rosterCache,

			ILogger<PlayerAddSource> logger,
			ToVersionedModelMapper toVersionedMapper,
			ToCoreDataMapper toCoreDataMapper,
			ProgramOptions programOptions,
			IDatabaseProvider dbProvider,
			DataDirectoryPath dataPath,
			IWebRequestClient webClient)
			: base(
				  logger,
				  toVersionedMapper,
				  toCoreDataMapper,
				  programOptions,
				  dbProvider,
				  dataPath,
				  webClient)
		{
			_rosterCache = rosterCache;
		}

		protected override bool SupportsFilePersistence => true;

		protected override string GetVersionedFilePath(string nflId)
		{
			return DataPath.Player(nflId);
		}

		protected override string GetSourceUri(string nflId)
		{
			return Endpoints.Page.PlayerProfile(nflId);
		}

		protected override Task OnVersionedModelMappedAsync(string nflId, PlayerAddVersionedModel versioned)
		{
			versioned.NflId = nflId;
			return Task.CompletedTask;
		}

		protected override async Task OnCoreDataMappedAsync(string nflId, PlayerAdd playerAdd)
		{
			(int? number, Position? position, RosterStatus? status)? data = await _rosterCache.GetPlayerDataAsync(nflId);

			if (data.HasValue)
			{
				playerAdd.Number = data.Value.number;
				playerAdd.Position = data.Value.position;
				playerAdd.Status = data.Value.status;
			}
		}
	}
}
