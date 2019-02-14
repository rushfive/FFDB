﻿using Microsoft.Extensions.Logging;
using R5.FFDB.Components.Pipelines.Stats;
using R5.FFDB.Components.Pipelines.Teams;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Database.DbContext;
using R5.FFDB.Core.Entities;
using R5.Lib.ExtensionMethods;
using R5.Lib.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Pipelines.Setup
{
	public class InitialSetupPipeline : Pipeline<InitialSetupPipeline.Context>
	{
		private ILogger<InitialSetupPipeline> _logger { get; }

		public InitialSetupPipeline(
			ILogger<InitialSetupPipeline> logger,
			AsyncPipelineStage<Context> head)
			: base(logger, head, "Initial Setup")
		{
			_logger = logger;
		}


		public class Context
		{
			
		}

		public static InitialSetupPipeline Create(IServiceProvider sp)
		{
			var initialize = sp.Create<Stages.Initialize>();
			var addTeams = sp.Create<Stages.AddTeams>();
			var addStats = sp.Create<Stages.AddStats>();
			var updateRosterMappings = sp.Create<Stages.UpdateRosterMappings>();

			AsyncPipelineStage<Context> chain = initialize;
			chain
				.SetNext(addTeams)
				.SetNext(addStats)
				.SetNext(updateRosterMappings);

			return sp.Create<InitialSetupPipeline>(chain);
		}

		public static class Stages
		{
			public class Initialize : Stage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }

				public Initialize(
					ILogger<Initialize> logger,
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
					ILogger<AddTeams> logger,
					IDatabaseProvider dbProvider)
					: base(logger, "Add Teams")
				{
					_dbProvider = dbProvider;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					List<Team> teams = TeamDataStore.GetAll();

					IDatabaseContext dbContext = _dbProvider.GetContext();
					await dbContext.Team.AddAllAsync(teams);

					return ProcessResult.Continue;
				}
			}

			public class AddStats : Stage<Context>
			{
				private IServiceProvider _serviceProvider { get; }

				public AddStats(
					ILogger<AddStats> logger,
					IServiceProvider serviceProvider)
					: base(logger, "Add Stats")
				{
					_serviceProvider = serviceProvider;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					var pipeline = UpdateMissingPipeline.Create(_serviceProvider);

					await pipeline.ProcessAsync(
						new UpdateMissingPipeline.Context());

					return ProcessResult.Continue;
				}
			}

			public class UpdateRosterMappings : Stage<Context>
			{
				private IServiceProvider _serviceProvider { get; }

				public UpdateRosterMappings(
					ILogger<UpdateRosterMappings> logger,
					IServiceProvider serviceProvider)
					: base(logger, "Update Roster Mappings")
				{
					_serviceProvider = serviceProvider;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					var pipeline = UpdateRosterMappingsPipeline.Create(_serviceProvider);

					await pipeline.ProcessAsync(
						new UpdateRosterMappingsPipeline.Context());

					return ProcessResult.Continue;
				}
			}
		}
	}
}