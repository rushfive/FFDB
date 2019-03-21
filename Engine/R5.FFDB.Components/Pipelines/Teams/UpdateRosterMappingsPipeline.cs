using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.Dynamic.Rosters;
using R5.FFDB.Components.Extensions;
using R5.FFDB.Components.Pipelines.CommonStages;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.Internals.Abstractions.Pipeline;
using R5.Internals.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Pipelines.Teams
{
	public class UpdateRosterMappingsPipeline : Pipeline<UpdateRosterMappingsPipeline.Context>
	{
		private IAppLogger _logger { get; }

		public UpdateRosterMappingsPipeline(
			IAppLogger logger,
			IServiceProvider serviceProvider)
			: base(logger, serviceProvider, "Update Rosters")
		{
			_logger = logger;
		}

		protected override List<Type> Stages => new List<Type>
		{
			typeof(Stage.ResolveNewRosteredPlayers),
			typeof(FetchPlayersStage<Context>),
			typeof(Stage.Update)
		};

		public class Context : IFetchPlayersContext
		{
			public List<string> FetchNflIds { get; set; }
		}

		public static class Stage
		{
			public class ResolveNewRosteredPlayers : Stage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }
				private IRosterCache _rosterCache { get; }

				public ResolveNewRosteredPlayers(
					IAppLogger logger,
					IDatabaseProvider dbProvider,
					IRosterCache rosterCache)
					: base(logger, "Resolve New Rostered Players")
				{
					_dbProvider = dbProvider;
					_rosterCache = rosterCache;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					IDatabaseContext dbContext = _dbProvider.GetContext();

					HashSet<string> existingPlayers = (await dbContext.Player.GetAllAsync())
						.Select(p => p.NflId)
						.ToHashSet(StringComparer.OrdinalIgnoreCase);

					List<string> newIds = (await _rosterCache.GetRosteredIdsAsync())
						.Where(id => !existingPlayers.Contains(id))
						.ToList();

					context.FetchNflIds = newIds;

					return ProcessResult.Continue;
				}
			}

			public class Update : Stage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }
				private IRosterCache _rosterCache { get; }

				public Update(
					IAppLogger logger,
					IDatabaseProvider dbProvider,
					IRosterCache rosterCache)
					: base(logger, "Update Rosters")
				{
					_dbProvider = dbProvider;
					_rosterCache = rosterCache;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					IDatabaseContext dbContext = _dbProvider.GetContext();

					List<Roster> rosters = await _rosterCache.GetAsync();

					await dbContext.Team.UpdateRosterMappingsAsync(rosters);

					return ProcessResult.Continue;
				}
			}
		}
	}

}
