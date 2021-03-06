﻿using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.Static.TeamStats;
using R5.FFDB.Components.CoreData.Static.WeekMatchups;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Models;
using R5.Internals.Abstractions.Pipeline;
using R5.Internals.Caching.Caches;
using R5.Internals.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Pipelines.Stats
{
	public class UpdateMissingPipeline : Pipeline<UpdateMissingPipeline.Context>
	{
		private IAppLogger _logger { get; }

		public UpdateMissingPipeline(
			IAppLogger logger,
			IServiceProvider serviceProvider)
			: base(logger, serviceProvider, "Add Stats for Missing Weeks")
		{
			_logger = logger;
		}

		public class Context
		{
			public List<WeekInfo> MissingWeeks { get; set; }
		}

		protected override List<Type> Stages => new List<Type>
		{
			typeof(Stage.GetMissingWeeks),
			typeof(Stage.AddMissingWeeks)
		};

		public static class Stage
		{
			public class GetMissingWeeks : Stage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }
				private AvailableWeeksValue _availableWeeks { get; }

				public GetMissingWeeks(
					IAppLogger logger,
					IDatabaseProvider dbProvider,
					AvailableWeeksValue availableWeeks)
					: base(logger, "Get Missing Weeks")
				{
					_dbProvider = dbProvider;
					_availableWeeks = availableWeeks;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					IDatabaseContext dbContext = _dbProvider.GetContext();

					HashSet<WeekInfo> alreadyUpdated = (await dbContext.UpdateLog.GetAsync())
						.ToHashSet();

					List<WeekInfo> missing = (await _availableWeeks.GetAsync())
						.Where(w => !alreadyUpdated.Contains(w))
						.ToList();

					if (!missing.Any())
					{
						LogInformation("Stats for all available weeks have already been added.");
						return ProcessResult.End;
					}

					context.MissingWeeks = missing;

					return ProcessResult.Continue;
				}
			}

			public class AddMissingWeeks : Stage<Context>
			{
				private IServiceProvider _serviceProvider { get; }
				private IAsyncLazyCache _cache { get; }

				public AddMissingWeeks(
					IAppLogger logger,
					IServiceProvider serviceProvider,
					IAsyncLazyCache cache)
					: base(logger, "Add Missing Weeks")
				{
					_serviceProvider = serviceProvider;
					_cache = cache;
				}

				public override async Task<ProcessStageResult> ProcessAsync(Context context)
				{
					LogDebug($"Adding stats for {context.MissingWeeks.Count} weeks.");

					foreach (var week in context.MissingWeeks)
					{
						var pipeline = _serviceProvider.Create<AddForWeekPipeline>();

						await pipeline.ProcessAsync(new AddForWeekPipeline.Context
						{
							Week = week
						});

						ClearWeekScopedCaches(week);

						LogDebug($"Finished adding stats for week '{week}'.");
					}

					return ProcessResult.Continue;
				}

				private void ClearWeekScopedCaches(WeekInfo week)
				{
					_cache.Remove(WeekMatchupsCache.CacheKey(week));
					_cache.Remove(TeamWeekStatsCache.CacheKey(week));
				}
			}
		}
	}
}
