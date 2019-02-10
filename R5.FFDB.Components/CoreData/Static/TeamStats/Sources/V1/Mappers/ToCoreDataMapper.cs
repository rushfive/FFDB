using R5.FFDB.Components.CoreData.Static.TeamStats.Sources.V1.Models;
using R5.FFDB.Core.Entities;
using R5.FFDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData.Static.TeamStats.Sources.V1.Mappers
{
	public class ToCoreDataMapper : IMapper<TeamStatsVersionedModel, List<TeamWeekStats>>
	{
		public List<TeamWeekStats> Map(TeamStatsVersionedModel model)
		{
			var home = MapStats(model.HomeTeamStats, model.Week);
			var away = MapStats(model.AwayTeamStats, model.Week);

			return new List<TeamWeekStats>
			{
				home,
				away
			};
		}

		private TeamWeekStats MapStats(TeamStatsVersionedModel.Stats model, WeekInfo week)
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
