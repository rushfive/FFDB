﻿using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.CoreData.Static.PlayerStats.Sources.V1.Models;
using R5.FFDB.Components.CoreData.Static.PlayerStats.Sources.V1.Mappers;
using R5.FFDB.Components.Http;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System.Collections.Generic;
using R5.FFDB.Core;

namespace R5.FFDB.Components.CoreData.Static.PlayerStats.Sources.V1
{
	public interface IPlayerWeekStatsSource : ICoreDataSource<List<PlayerWeekStats>, WeekInfo> { }

	public class PlayerWeekStatsSource : CoreDataSource<PlayerWeekStatsVersioned, List<PlayerWeekStats>, WeekInfo>, IPlayerWeekStatsSource
	{
		public PlayerWeekStatsSource(
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

		protected override bool SupportsSourceFilePersistence => true;
		protected override bool SupportsVersionedFilePersistence => true;
		protected override bool SupportsDataRepoFetch => true;

		protected override string GetVersionedFilePath(WeekInfo week)
		{
			return DataPath.Versioned.V1.PlayerWeekStats(week);
		}

		protected override string GetSourceFilePath(WeekInfo week)
		{
			return DataPath.SourceFiles.V1.PlayerWeekStats(week);
		}

		protected override string GetSourceUri(WeekInfo week)
		{
			return Endpoints.Api.WeekStats(week);
		}

		protected override string GetDataRepoUri(WeekInfo week)
		{
			return $"https://raw.githubusercontent.com/rushfive/FFDB.Data/master/versioned/player_week_stats/{week}.json";
		}
	}
}
