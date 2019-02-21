using Microsoft.Extensions.Logging;
using R5.FFDB.Components.CoreData.Static.TeamStats;
using R5.FFDB.Components.CoreData.Static.WeekMatchups;
using R5.FFDB.Components.ValueProviders;
using R5.FFDB.Core.Database;
using R5.FFDB.Core.Models;
using R5.Lib.Cache.AsyncLazyCache;
using R5.Lib.ExtensionMethods;
using R5.Lib.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.FFDB.Components.Pipelines.Stats
{
	public class UpdateMissingPipeline : Pipeline<UpdateMissingPipeline.Context>
	{
		private ILogger<UpdateMissingPipeline> _logger { get; }

		public UpdateMissingPipeline(
			ILogger<UpdateMissingPipeline> logger,
			AsyncPipelineStage<Context> head)
			: base(logger, head, "Add Stats for Missing Weeks")
		{
			_logger = logger;
		}

		public class Context
		{
			public List<WeekInfo> MissingWeeks { get; set; }
		}

		public static UpdateMissingPipeline Create(IServiceProvider sp)
		{
			var getMissingWeeks = sp.Create<Stages.GetMissingWeeks>();
			var addMissingWeeks = sp.Create<Stages.AddMissingWeeks>();

			AsyncPipelineStage<Context> chain = getMissingWeeks;
			chain.SetNext(addMissingWeeks);

			return sp.Create<UpdateMissingPipeline>(chain);
		}

		public static class Stages
		{
			public class GetMissingWeeks : Stage<Context>
			{
				private IDatabaseProvider _dbProvider { get; }
				private AvailableWeeksValue _availableWeeks { get; }

				public GetMissingWeeks(
					ILogger<GetMissingWeeks> logger,
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
					ILogger<AddMissingWeeks> logger,
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
						var pipeline = AddForWeekPipeline.Create(_serviceProvider, nestedDepth: 1);

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
