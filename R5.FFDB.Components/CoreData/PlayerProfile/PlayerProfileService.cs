using R5.FFDB.Components.CoreData.PlayerProfile.Models;
using R5.FFDB.Components.CoreData.PlayerProfile.Values;
using System.Collections.Generic;

namespace R5.FFDB.Components.CoreData.PlayerProfile
{
	public interface IPlayerProfileService
	{
		List<Core.Models.PlayerProfile> Get();
	}

	public class PlayerProfileService : IPlayerProfileService
	{
		private PlayerProfilesValue _playerProfiles { get; }

		public PlayerProfileService(
			PlayerProfilesValue playerProfiles)
		{
			_playerProfiles = playerProfiles;
		}

		public List<Core.Models.PlayerProfile> Get()
		{
			return _playerProfiles.Get();
		}
	}
}
