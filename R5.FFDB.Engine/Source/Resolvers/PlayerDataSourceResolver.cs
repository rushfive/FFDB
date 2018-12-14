using R5.FFDB.Components.CoreData.PlayerProfile;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Engine.Source.Resolvers
{
	public interface IPlayerDataSourceResolver : ISourceResolver<IPlayerProfileSource>
	{

	}

	public class PlayerDataSourceResolver : SourceResolver<IPlayerProfileSource>, IPlayerDataSourceResolver
	{
		protected override string SourceName => "Player Data";

		public PlayerDataSourceResolver(
			PlayerProfileSource source)
			: base(new List<IPlayerProfileSource> { source })
		{
		}
	}
}