using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.CoreData.Dynamic.Rosters;
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
	public interface IPlayerAddSource : ICoreDataSource<PlayerAdd, string> { }

	public class PlayerAddSource : CoreDataSource<PlayerAddVersioned, PlayerAdd, string>, IPlayerAddSource
	{
		private IRosterCache _rosterCache { get; }

		public PlayerAddSource(
			IRosterCache rosterCache,
			ILogger<PlayerAddSource> logger,
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
		{
			_rosterCache = rosterCache;
		}

		protected override bool SupportsSourceFilePersistence => false;
		protected override bool SupportsVersionedFilePersistence => true;

		protected override string GetVersionedFilePath(string nflId)
		{
			return DataPath.Versioned.V1.PlayerAdd(nflId);
		}

		protected override string GetSourceFilePath(string key)
		{
			return DataPath.SourceFiles.V1.PlayerAdd(key);
		}

		protected override string GetSourceUri(string nflId)
		{
			return Endpoints.Page.PlayerProfile(nflId);
		}
	}
}
