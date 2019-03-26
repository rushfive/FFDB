using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1.Mappers;
using R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1.Models;
using R5.FFDB.Components.Http;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1
{
	public interface IRosterSource : ICoreDataSource<Roster, Team>
	{
		string GetVersionedFilePath(Team team);
	}

	public class RosterSource : CoreDataSource<RosterVersioned, Roster, Team>, IRosterSource
	{
		public RosterSource(
			IAppLogger logger,
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

		string IRosterSource.GetVersionedFilePath(Team team)
		{
			return GetVersionedFilePath(team);
		}

		protected override bool SupportsSourceFilePersistence => false;
		protected override bool SupportsVersionedFilePersistence => true;
		protected override bool SupportsDataRepoFetch => false;

		protected override string GetVersionedFilePath(Team team)
		{
			return DataPath.Versioned.V1.Roster(team);
		}

		protected override string GetSourceFilePath(Team team)
		{
			return null;
		}

		protected override string GetSourceUri(Team team)
		{
			return Endpoints.Page.TeamRoster(team);
		}

		protected override string GetDataRepoUri(Team key)
		{
			return null;
		}
	}
}
