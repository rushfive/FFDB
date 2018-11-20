using R5.FFDB.Components.WeekStats;
using R5.FFDB.Components.WeekStats.Sources.NFLFantasyApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Engine.SourceResolvers
{
	public interface IWeekStatsSourceResolver : ISourceResolver<IWeekStatsSource>
	{

	}

	public class WeekStatsSourceResolver : SourceResolver<IWeekStatsSource>, IWeekStatsSourceResolver
	{
		protected override string SourceName => "Week Stats";

		public WeekStatsSourceResolver(
			WeekStatsSource source)
			: base(new List<IWeekStatsSource> { source })
		{
		}
	}
}
