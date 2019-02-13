using Microsoft.Extensions.Logging;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.Core.Entities;
using System.Collections.Generic;
using System.Linq;
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
		//private IPlayerSource _playerSource { get; }
		//private IPlayerService _playerService { get; }
		//private RostersValue _rostersValue { get; }

		public ProcessorHelper(
			ILogger<ProcessorHelper> logger
			//IPlayerSource playerSource,
			//IPlayerService playerService,
			//RostersValue rostersValue
			)
		{
			_logger = logger;
			//_playerSource = playerSource;
			//_playerService = playerService;
			//_rostersValue = rostersValue;
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

			//await _playerSource.FetchAsync(newIds);

			List<Player> playerProfiles = null;// _playerService.Get(newIds);
			if (!playerProfiles.Any())
			{
				_logger.LogInformation("No player profiles resolved from files. Skipping database update.");
				return;
			}

			List<Roster> rosters = null;// await _rostersValue.GetAsync();

			//await dbContext.Player.AddAsync(playerProfiles, rosters);
		}
	}
}
