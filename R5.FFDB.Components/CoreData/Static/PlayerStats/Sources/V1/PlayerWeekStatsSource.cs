using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.CoreData.Static.PlayerStats.Sources.V1.Models;
using R5.FFDB.Components.CoreData.Static.PlayerStats.Sources.V1.Mappers;
using R5.FFDB.Components.Http;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.PlayerStats.Sources.V1
{
	public interface IPlayerWeekStatsSource : ICoreDataSource<List<PlayerWeekStats>, WeekInfo> { }

	public class PlayerWeekStatsSource : CoreDataSource<PlayerWeekStatsVersioned, List<PlayerWeekStats>, WeekInfo>, IPlayerWeekStatsSource
	{
		public PlayerWeekStatsSource(
			ILogger<PlayerWeekStatsSource> logger,
			IToVersionedMapper toVersionedMapper,
			IToCoreMapper toCoreMapper,
			ProgramOptions programOptions,
			IDatabaseProvider dbProvider,
			DataDirectoryPath dataPath,
			IWebRequestClient webClient)
			: base(
				  logger,
				  toVersionedMapper,
				  toCoreMapper,
				  programOptions,
				  dbProvider,
				  dataPath,
				  webClient)
		{

		}

		protected override bool SupportsFilePersistence => true;

		protected override string GetVersionedFilePath(WeekInfo week)
		{
			return DataPath.PlayerWeekStats(week);
		}

		protected override string GetSourceUri(WeekInfo week)
		{
			return Endpoints.Api.WeekStats(week);
		}

		protected override Task OnVersionedModelMappedAsync(WeekInfo week, PlayerWeekStatsVersioned versioned)
		{
			return Task.CompletedTask;
		}

		protected override Task OnCoreDataMappedAsync(WeekInfo week, List<PlayerWeekStats> stats)
		{
			return Task.CompletedTask;
		}
	}
}
