using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.PlayerProfile;
using R5.FFDB.Components.CoreData.Roster.Values;
using R5.FFDB.Core.Models;
using R5.FFDB.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Engine.Processors
{
	public interface IProcessorHelper
	{
		Task AddPlayerProfilesAsync(List<string> nflIds, IDatabaseContext dbContext);
	}

	internal class ProcessorHelper : IProcessorHelper
	{
		private ILogger<ProcessorHelper> _logger { get; }
		private IPlayerProfileSource _profileSource { get; }
		private IPlayerProfileService _profileService { get; }
		private RostersValue _rostersValue { get; }

		public ProcessorHelper(
			ILogger<ProcessorHelper> logger,
			IPlayerProfileSource profileSource,
			IPlayerProfileService profileService,
			RostersValue rostersValue)
		{
			_logger = logger;
			_profileSource = profileSource;
			_profileService = profileService;
			_rostersValue = rostersValue;
		}

		public async Task AddPlayerProfilesAsync(List<string> nflIds, IDatabaseContext dbContext)
		{
			HashSet<string> existingIds = (await dbContext.Player.GetAllAsync())
				.Select(p => p.NflId)
				.ToHashSet();

			List<string> newIds = nflIds.Where(id => !existingIds.Contains(id)).ToList();
			if (!newIds.Any())
			{
				_logger.LogInformation($"No new player profiles to add. "
					+ $"The {nflIds.Count} players already exist in the database.");
				return;
			}

			_logger.LogInformation($"Adding {newIds.Count} player profiles to database.");

			await _profileSource.FetchAsync(newIds);

			List<PlayerProfile> playerProfiles = _profileService.Get(newIds);
			if (!playerProfiles.Any())
			{
				_logger.LogInformation("No player profiles resolved from files. Skipping database update.");
				return;
			}

			List<Roster> rosters = await _rostersValue.GetAsync();

			await dbContext.Player.UpdateAsync(playerProfiles, rosters);
		}
	}
}
