using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.CoreData.Static.ReceiverTargets.Sources.V1.Models;
using R5.FFDB.Components.CoreData.Static.ReceiverTargets.Sources.V1.Mappers;
using R5.FFDB.Components.Http;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.ReceiverTargets.Sources.V1
{
	// This source exists because the current source for player week stats (NFL Fantasy API) doesn't
	// include targets for players.
	// This source will take a gameId, and return a map of nflIds->targets

	public interface IReceiverTargetsSource : ICoreDataSource<Dictionary<string, int>, string> { }

	public class ReceiverTargetsSource : CoreDataSource<ReceiverTargetsVersioned, Dictionary<string, int>, string>, IReceiverTargetsSource
	{
		public ReceiverTargetsSource(
			ILogger<ReceiverTargetsSource> logger,
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

		}

		protected override bool SupportsFilePersistence => true;

		protected override string GetVersionedFilePath(string gameId)
		{
			return DataPath.ReceiverTargets(gameId);
		}

		protected override string GetSourceUri(string gameId)
		{
			return Endpoints.Page.ReceiverTargets(gameId);
		}
	}
}
