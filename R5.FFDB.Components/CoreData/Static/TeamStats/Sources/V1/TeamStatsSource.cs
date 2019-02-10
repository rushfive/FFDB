﻿using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.CoreData.Static.TeamStats.Sources.V1.Models;
using R5.FFDB.Components.CoreData.Static.TeamStats.Sources.V1.Mappers;
using R5.FFDB.Components.Http;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using R5.FFDB.Components.CoreData.Static.TeamStats.Models;

namespace R5.FFDB.Components.CoreData.Static.TeamStats.Sources.V1
{
	public interface ITeamStatsSource : ICoreDataSource<TeamStatsSourceModel, (string gameId, WeekInfo week)> { }

	public class TeamStatsSource : CoreDataSource<TeamStatsVersioned, TeamStatsSourceModel, (string, WeekInfo)>, ITeamStatsSource
	{
		public TeamStatsSource(
			ILogger<TeamStatsSource> logger,
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
		{

		}

		protected override bool SupportsFilePersistence => true;

		protected override string GetVersionedFilePath((string, WeekInfo) gameWeek)
		{
			return DataPath.TeamStats(gameWeek.Item1);
		}

		protected override string GetSourceUri((string, WeekInfo) gameWeek)
		{
			return Endpoints.Api.GameCenterStats(gameWeek.Item1);
		}

		protected override Task OnVersionedModelMappedAsync((string, WeekInfo) gameWeek, TeamStatsVersioned versioned)
		{
			return Task.CompletedTask;
		}

		protected override Task OnCoreDataMappedAsync((string, WeekInfo) gameWeek, TeamStatsSourceModel stats)
		{
			return Task.CompletedTask;
		}
	}
}