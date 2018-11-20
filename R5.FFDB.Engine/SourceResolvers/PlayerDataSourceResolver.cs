using Microsoft.Extensions.Logging;
using R5.FFDB.Components.PlayerData;
using R5.FFDB.Components.PlayerData.Sources.NFLWebPlayerProfile;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Engine.SourceResolvers
{
	public interface IPlayerDataSourceResolver : ISourceResolver<IPlayerDataSource>
	{

	}

	public class PlayerDataSourceResolver : SourceResolver<IPlayerDataSource>, IPlayerDataSourceResolver
	{
		protected override string SourceName => "Player Data";

		public PlayerDataSourceResolver(
			PlayerDataSource source)
			: base(new List<IPlayerDataSource> { source })
		{
		}
	}
}