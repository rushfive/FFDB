using R5.FFDB.Components.PlayerTeamHistory;
using R5.FFDB.Components.PlayerTeamHistory.Sources.NFLWeb;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Engine.Source.Resolvers
{
	public interface IPlayerTeamHistorySourceResolver : ISourceResolver<IPlayerTeamHistorySource>
	{

	}

	public class PlayerTeamHistorySourceResolver : SourceResolver<IPlayerTeamHistorySource>, IPlayerTeamHistorySourceResolver
	{
		protected override string SourceName => "Player Team History";

		public PlayerTeamHistorySourceResolver(
			PlayerTeamHistorySource source)
			: base(new List<IPlayerTeamHistorySource> { source })
		{
		}
	}
}
