using R5.FFDB.Components.CoreData.Roster;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Engine.Source.Resolvers
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
