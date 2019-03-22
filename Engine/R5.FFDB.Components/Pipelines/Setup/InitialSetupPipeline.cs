using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Pipelines.Stats;
using R5.FFDB.Components.Pipelines.Teams;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Entities;
using R5.Internals.Abstractions.Pipeline;
using R5.Internals.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Pipelines.Setup
{
	public class InitialSetupPipeline : Pipeline<InitialSetupPipeline.Context>
	{
		private IAppLogger _logger { get; }

		public InitialSetupPipeline(
			IAppLogger logger,
			IServiceProvider serviceProvider)
			: base(logger, serviceProvider, "Initial Setup")
		{
			_logger = logger;
		}


		public class Context
		{
			public bool SkipAddingStats { get; set; }
		}

		protected override List<Type> Stages => new List<Type>
		{
			typeof(Stage.Initialize),
			typeof(Stage.AddTeams),
			typeof(Stage.AddStats),
			typeof(Stage.UpdateRosterMappings)
		};

		public static class Stage
		{
			public class Initialize : Stage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }

				public Initialize(
					IAppLogger logger,
					IDatabaseProvider dbProvider)
					: base(logger, "Initialize Database")
				{
					_dbProvider = dbProvider;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					LogInformation($"Will run using database provider '{_dbProvider.GetType().Name}'.");

					await _dbProvider.GetContext().InitializeAsync();

					return ProcessResult.Continue;
				}
			}

			public class AddTeams : Stage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }

				public AddTeams(
					IAppLogger logger,
					IDatabaseProvider dbProvider)
					: base(logger, "Add Teams")
				{
					_dbProvider = dbProvider;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					IDatabaseContext dbContext = _dbProvider.GetContext();
					
					List<int> existingTeams = await dbContext.Team.GetExistingTeamIdsAsync();
					
					List<Team> missing = Core.Teams.GetAll()
						.Where(t => existingTeams.Contains(t.Id))
						.ToList();

					if (!missing.Any())
					{
						LogInformation("All teams have already been added.");
						return ProcessResult.Continue;
					}
					
					await dbContext.Team.AddAsync(missing);

					return ProcessResult.Continue;
				}
			}

			public class AddStats : Stage<Context>
			{
				private IServiceProvider _serviceProvider { get; }

				public AddStats(
					IAppLogger logger,
					IServiceProvider serviceProvider)
					: base(logger, "Add Stats")
				{
					_serviceProvider = serviceProvider;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					if (context.SkipAddingStats)
					{
						LogInformation($"Program set to skip adding of stats, will not add.");
						return ProcessResult.Continue;
					}

					var pipeline = _serviceProvider.Create<UpdateMissingPipeline>();

					await pipeline.ProcessAsync(
						new UpdateMissingPipeline.Context());

					return ProcessResult.Continue;
				}
			}

			public class UpdateRosterMappings : Stage<Context>
			{
				private IServiceProvider _serviceProvider { get; }

				public UpdateRosterMappings(
					IAppLogger logger,
					IServiceProvider serviceProvider)
					: base(logger, "Update Roster Mappings")
				{
					_serviceProvider = serviceProvider;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					if (context.SkipAddingStats)
					{
						LogInformation($"Program set to skip adding of stats, will not update roster mappings.");
						return ProcessResult.Continue;
					}

					var pipeline = _serviceProvider.Create<UpdateRosterMappingsPipeline>();

					await pipeline.ProcessAsync(
						new UpdateRosterMappingsPipeline.Context());

					return ProcessResult.Continue;
				}
			}
		}
	}
}
