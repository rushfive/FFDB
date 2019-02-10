using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.CoreData.Static.WeekMatchups.Sources.V1.Mappers;
using R5.FFDB.Components.CoreData.Static.WeekMatchups.Sources.V1.Models;
using R5.FFDB.Components.Http;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.WeekMatchups.Sources.V1
{
	public interface IWeekMatchupSource : ICoreDataSource<List<WeekGameMatchup>, WeekInfo> { }

	public class WeekMatchupSource : CoreDataSource<WeekMatchupsVersionedModel, List<WeekGameMatchup>, WeekInfo>, IWeekMatchupSource
	{
		public WeekMatchupSource(
			ILogger<WeekMatchupSource> logger,
			IToVersionedModelMapper toVersionedMapper,
			IToCoreDataMapper toCoreDataMapper,
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
		{ }

		protected override bool SupportsFilePersistence => true;

		protected override string GetVersionedFilePath(WeekInfo week)
		{
			return DataPath.WeekGameMap(week);
		}

		protected override string GetSourceUri(WeekInfo week)
		{
			return Endpoints.Api.ScoreStripWeekGames(week.Season, week.Week);
		}

		protected override Task OnVersionedModelMappedAsync(WeekInfo week, WeekMatchupsVersionedModel versioned)
		{
			return Task.CompletedTask;
		}

		protected override Task OnCoreDataMappedAsync(WeekInfo key, List<WeekGameMatchup> coreData)
		{
			return Task.CompletedTask;
		}
	}
}
