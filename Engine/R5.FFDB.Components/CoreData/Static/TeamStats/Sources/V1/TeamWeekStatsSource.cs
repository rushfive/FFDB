using R5.FFDB.Components.Configurations;
using R5.FFDB.Components.CoreData.Static.TeamStats.Sources.V1.Models;
using R5.FFDB.Components.CoreData.Static.TeamStats.Sources.V1.Mappers;
using R5.FFDB.Components.Http;
using R5.FFDB.Core.Models;
using R5.FFDB.Components.CoreData.Static.TeamStats.Models;

namespace R5.FFDB.Components.CoreData.Static.TeamStats.Sources.V1
{
	public interface ITeamWeekStatsSource : ICoreDataSource<TeamWeekStatsSourceModel, (string gameId, WeekInfo week)> { }

	public class TeamWeekStatsSource : CoreDataSource<TeamWeekStatsVersioned, TeamWeekStatsSourceModel, (string, WeekInfo)>, ITeamWeekStatsSource
	{
		public TeamWeekStatsSource(
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

		protected override string GetVersionedFilePath((string, WeekInfo) gameWeek)
		{
			return DataPath.Versioned.V1.TeamStats(gameWeek.Item1);
		}

		protected override string GetSourceFilePath((string, WeekInfo) gameWeek)
		{
			return DataPath.SourceFiles.V1.TeamStats(gameWeek.Item1);
		}

		protected override string GetSourceUri((string, WeekInfo) gameWeek)
		{
			return Endpoints.Api.GameCenterStats(gameWeek.Item1);
		}

		protected override string GetDataRepoUri((string, WeekInfo) gameWeek)
		{
			return $"https://raw.githubusercontent.com/rushfive/FFDB.Data/master/versioned/team_stats/{gameWeek.Item1}.json";
		}
	}
}
