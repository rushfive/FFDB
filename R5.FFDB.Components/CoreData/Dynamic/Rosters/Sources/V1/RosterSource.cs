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
	// TODO: need a configurable way to determine when
	// saved roster files are too old (requiring a re-fetch)
	public interface IRosterSource : ICoreDataSource<Roster, Team> { }

	public class RosterSource : CoreDataSource<RosterVersionedModel, Roster, Team>, IRosterSource
	{
		public RosterSource(
			ILogger<RosterSource> logger,
			IToVersionedModelMapper toVersionedMapper,
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

		}

		protected override bool SupportsFilePersistence => true;

		protected override string GetVersionedFilePath(Team team)
		{
			return DataPath.Roster(team);
		}

		protected override string GetSourceUri(Team team)
		{
			return Endpoints.Page.TeamRoster(team.ShortName, team.Abbreviation);
		}

		protected override Task OnVersionedModelMappedAsync(Team team, RosterVersionedModel versioned)
		{
			return Task.CompletedTask;
		}

		protected override Task OnCoreDataMappedAsync(Team team, Roster roster)
		{
			return Task.CompletedTask;
		}
	}
}
