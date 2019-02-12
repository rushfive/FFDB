using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1.Mappers;
using R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1.Models;
using R5.FFDB.Components.Http;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1
{
	public interface IRosterSource : ICoreDataSource<Roster, Team> { }

	public class RosterSource : CoreDataSource<RosterVersioned, Roster, Team>, IRosterSource
	{
		public RosterSource(
			ILogger<RosterSource> logger,
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

		protected override string GetVersionedFilePath(Team team)
		{
			return DataPath.Roster(team);
		}

		protected override string GetSourceUri(Team team)
		{
			return Endpoints.Page.TeamRoster(team);
		}
	}
}
