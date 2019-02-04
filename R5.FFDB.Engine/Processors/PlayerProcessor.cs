using R5.FFDB.Components.Pipelines.Players;
using System;
using System.Threading.Tasks;

namespace R5.FFDB.Engine.Processors
{
	public class PlayerProcessor
	{
		private IServiceProvider _serviceProvider { get; }

		public PlayerProcessor(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public Task UpdateCurrentlyRosteredAsync()
		{
			var context = new UpdateCurrentlyRosteredPipeline.Context();

			var pipeline = UpdateCurrentlyRosteredPipeline.Create(_serviceProvider);

			return pipeline.ProcessAsync(context);


			//_logger.LogInformation("Updating players that are currently rostered on a team.");

			//IDatabaseContext dbContext = _dbProvider.GetContext();

			//List<Roster> rosters = await _rostersValue.GetAsync();
			//List<string> nflIds = rosters.SelectMany(r => r.Players).Select(p => p.NflId).ToList();

			//await _playerSource.FetchAsync(nflIds, overwriteExisting: false);

			//List<Player> players = (await dbContext.Player.GetAllAsync())
			//	.Where(p => nflIds.Contains(p.NflId))
			//	.ToList();

			//// todo: first add players that currently dont exist in db?
			//// then, run an update on the diff

			//await dbContext.Player.UpdateAsync(players, rosters);

			//_logger.LogInformation("Successfully updated players currently rostered on a team.");
		}

		public Task UpdateAllExistingAsync()
		{
			var context = new UpdateAllPipeline.Context();

			var pipeline = UpdateAllPipeline.Create(_serviceProvider);

			return pipeline.ProcessAsync(context);

			//_logger.LogInformation("Updating all players currently existing in database.");

			//IDatabaseContext dbContext = _dbProvider.GetContext();

			//List<Player> players = await dbContext.Player.GetAllAsync();
			//List<string> nflIds = players.Select(p => p.NflId).ToList();

			//await _playerSource.FetchAsync(nflIds, overwriteExisting: true);

			//List<Roster> rosters = await _rostersValue.GetAsync();

			//await dbContext.Player.UpdateAsync(players, rosters);

			//_logger.LogInformation("Successfully updated all players currently existing in database.");
		}
	}
}
