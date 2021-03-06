﻿using R5.FFDB.Components.CoreData.Static.TeamStats.Models;
using R5.FFDB.Components.CoreData.Static.TeamStats.Sources.V1.Models;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System.Threading.Tasks;

namespace R5.FFDB.Components.CoreData.Static.TeamStats.Sources.V1.Mappers
{
	public interface IToCoreMapper : IAsyncMapper<TeamWeekStatsVersioned, TeamWeekStatsSourceModel, (string gameId, WeekInfo week)> { }

	public class ToCoreMapper : IToCoreMapper
	{
		public Task<TeamWeekStatsSourceModel> MapAsync(TeamWeekStatsVersioned model, (string, WeekInfo) gameWeek)
		{
			var home = MapStats(model.HomeTeamStats, model.Week);
			var away = MapStats(model.AwayTeamStats, model.Week);

			return Task.FromResult(new TeamWeekStatsSourceModel
			{
				HomeTeamStats = home,
				AwayTeamStats = away
			});
		}

		private TeamWeekStats MapStats(TeamWeekStatsVersioned.Stats model, WeekInfo week)
		{
			return new TeamWeekStats
			{
				TeamId = model.Id,
				Week = week,
				PlayerNflIds = model.PlayerNflIds,
				PointsFirstQuarter = model.PointsFirstQuarter,
				PointsSecondQuarter = model.PointsSecondQuarter,
				PointsThirdQuarter = model.PointsThirdQuarter,
				PointsFourthQuarter = model.PointsFourthQuarter,
				PointsOverTime = model.PointsOverTime,
				PointsTotal = model.PointsTotal,
				FirstDowns = model.FirstDowns,
				TotalYards = model.TotalYards,
				PassingYards = model.PassingYards,
				RushingYards = model.RushingYards,
				Penalties = model.Penalties,
				PenaltyYards = model.PenaltyYards,
				Turnovers = model.Turnovers,
				Punts = model.Punts,
				PuntYards = model.PuntYards,
				TimeOfPossessionSeconds = model.TimeOfPossessionSeconds
			};
		}
	}
}
