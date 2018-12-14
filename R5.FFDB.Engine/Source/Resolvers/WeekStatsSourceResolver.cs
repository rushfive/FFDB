using R5.FFDB.Components.CoreData.WeekStats;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Engine.Source.Resolvers
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
