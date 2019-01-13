using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.Players;
using R5.FFDB.Components.CoreData.Rosters.Values;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R5.FFDB.Engine.Processors
{
	public class PlayerProcessor
	{
		private ILogger<PlayerProcessor> _logger { get; }
		private IDatabaseProvider _dbProvider { get; }
		private RostersValue _rostersValue { get; }
		private IPlayerSource _playerSource { get; }
		private IPlayerService _playerService { get; }

		public PlayerProcessor(
			ILogger<PlayerProcessor> logger,
			IDatabaseProvider dbProvider,
			RostersValue rostersValue,
			IPlayerSource playerSource,
			IPlayerService playerService)
		{
			_logger = logger;
			_dbProvider = dbProvider;
			_rostersValue = rostersValue;
			_playerSource = playerSource;
			_playerService = playerService;
		}

		public async Task UpdateCurrentlyRosteredAsync()
		{
			_logger.LogInformation("Updating players that are currently rostered on a team.");

			IDatabaseContext dbContext = _dbProvider.GetContext();

			List<Roster> rosters = await _rostersValue.GetAsync();
			List<string> nflIds = rosters.SelectMany(r => r.Players).Select(p => p.NflId).ToList();

			await _playerSource.FetchAsync(nflIds, overwriteExisting: false);

			List<Player> players = (await dbContext.Player.GetAllAsync())
				.Where(p => nflIds.Contains(p.NflId))
				.ToList();

			// todo: first add players that currently dont exist in db?
			// then, run an update on the diff

			await dbContext.Player.UpdateAsync(players, rosters);

			_logger.LogInformation("Successfully updated players currently rostered on a team.");
		}

		public async Task UpdateAllExistingAsync()
		{
			_logger.LogInformation("Updating all players currently existing in database.");

			IDatabaseContext dbContext = _dbProvider.GetContext();

			List<Player> players = await dbContext.Player.GetAllAsync();
			List<string> nflIds = players.Select(p => p.NflId).ToList();

			await _playerSource.FetchAsync(nflIds, overwriteExisting: true);

			List<Roster> rosters = await _rostersValue.GetAsync();

			await dbContext.Player.UpdateAsync(players, rosters);

			_logger.LogInformation("Successfully updated all players currently existing in database.");
		}
	}
}
