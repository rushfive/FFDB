using R5.FFDB.Components.Roster;
using R5.FFDB.Components.Roster.Sources.NFLWebTeam;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Engine.SourceResolvers
{
	public interface IRosterSourceResolver : ISourceResolver<IRosterSource>
	{

	}

	public class RosterSourceResolver : SourceResolver<IRosterSource>, IRosterSourceResolver
	{
		protected override string SourceName => "Roster";

		public RosterSourceResolver(
			RosterSource source)
			: base(new List<IRosterSource> { source })
		{
		}
	}
}
