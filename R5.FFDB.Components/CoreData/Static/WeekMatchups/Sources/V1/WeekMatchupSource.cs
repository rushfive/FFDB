﻿using Microsoft.Extensions.Logging;
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

	public class WeekMatchupSource : CoreDataSource<WeekMatchupsVersioned, List<WeekGameMatchup>, WeekInfo>, IWeekMatchupSource
	{
		public WeekMatchupSource(
			ILogger<WeekMatchupSource> logger,
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

		protected override bool SupportsSourceFilePersistence => true;
		protected override bool SupportsVersionedFilePersistence => true;

		protected override string GetVersionedFilePath(WeekInfo week)
		{
			return DataPath.Versioned.V1.WeekMatchup(week);
		}

		protected override string GetSourceFilePath(WeekInfo week)
		{
			return DataPath.SourceFiles.V1.WeekMatchup(week);
		}

		protected override string GetSourceUri(WeekInfo week)
		{
			return Endpoints.Api.ScoreStripWeekGames(week);
		}
	}
}
